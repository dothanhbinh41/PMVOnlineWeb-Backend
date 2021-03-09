using Microsoft.AspNetCore.Authorization;
using PMVOnline.Departments;
using PMVOnline.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using STask = System.Threading.Tasks.Task;

namespace PMVOnline.Tasks
{
    public class TaskAppService : ApplicationService, ITaskAppService
    {
        readonly IRepository<Task, ulong> taskRepository;
        readonly IRepository<TaskAction, Guid> taskActionRepository;
        readonly IRepository<TaskComment, Guid> taskCommentRepository;
        readonly IRepository<TaskFollow, Guid> taskFollowRepostiory;
        readonly IRepository<IdentityUser, Guid> appUserRepository;
        readonly IRepository<IdentityRole, Guid> roleRepository;
        private readonly IdentityUserManager identityUserManager;

        public TaskAppService(
            IRepository<Task, ulong> taskRepository,
            IRepository<TaskAction, Guid> taskActionRepository,
            IRepository<TaskComment, Guid> taskCommentRepository,
            IRepository<TaskFollow, Guid> taskFollowRepostiory,
            IRepository<IdentityUser, Guid> appUserRepository,
            IRepository<IdentityRole, Guid> roleRepository,
            IdentityUserManager identityUserManager
            )
        {
            this.taskRepository = taskRepository;
            this.taskActionRepository = taskActionRepository;
            this.taskCommentRepository = taskCommentRepository;
            this.taskFollowRepostiory = taskFollowRepostiory;
            this.appUserRepository = appUserRepository;
            this.roleRepository = roleRepository;
            this.identityUserManager = identityUserManager;
        }

        public async Task<TaskDto> CreateTask(CreateTaskRequestDto request)
        {
            var task = ObjectMapper.Map<CreateTaskRequestDto, Task>(request);

            var assignee = await appUserRepository.GetAsync(request.Assignee);
            var result = await taskRepository.InsertAsync(task);
            var id = CurrentUser.Id.Value;
            if (result != null)
            {
                await taskActionRepository.InsertAsync(new TaskAction { TaskId = task.Id, ActorId = id, Action = ActionType.CreateTask });
            }

            return ObjectMapper.Map<Task, TaskDto>(result);
        }

        public async Task<bool> FinishTask(FinishTaskRequest request)
        {
            var task = await taskRepository.GetAsync(d => d.Id == request.Id);
            if (task.Status != Status.Approved)
            {
                return false;
            }

            task.Status = request.Completed ? Status.Completed : Status.Incompleted;
            if (request.CompletedDate.HasValue && request.Completed)
            {
                task.CompletedDate = request.CompletedDate;
            }

            var updated = await taskRepository.UpdateAsync(task);
            await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.Id, ActorId = CurrentUser.Id, Action = request.Completed ? ActionType.CompletedTask : ActionType.IncompletedTask, Note = request.Note });
            return true;
        }

        public async Task<bool> FollowTask(FollowTaskRequest request)
        {
            var task = await taskFollowRepostiory.FirstOrDefaultAsync(d => d.TaskId == request.Id && d.UserId == CurrentUser.Id.Value);
            if (task == null)
            {
                await taskFollowRepostiory.InsertAsync(new TaskFollow { TaskId = request.Id, UserId = CurrentUser.Id.Value, Followed = true });
            }
            else
            {
                task.Followed = request.Follow;
                await taskFollowRepostiory.UpdateAsync(task);
            }
            await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.Id, ActorId = CurrentUser.Id, Action = request.Follow ? ActionType.Follow : ActionType.Unfollow });
            return true;
        }

        public async Task<UserDto[]> GetAllMember(Target target)
        {
            string dep = RoleName.Admin;
            if (target == Target.Other)
            {
                var users = (await appUserRepository.WithDetailsAsync(d => d.Roles)).Where(d => d.Roles != null && d.Roles.Count > 0).ToArray();
                return ObjectMapper.Map<IdentityUser[], UserDto[]>(users);
            }

            switch (target)
            {
                case Target.BuyCommodity:
                    dep = RoleName.Buy;
                    break;
                case Target.BuyOther:
                    dep = RoleName.Admin;
                    break;
                case Target.Make:
                case Target.Storage:
                    dep = RoleName.Storage;
                    break;
                case Target.Payment:
                    dep = RoleName.Account;
                    break;

            }

            var users2 = (await identityUserManager.GetUsersInRoleAsync(dep)).ToArray();
            return ObjectMapper.Map<IdentityUser[], UserDto[]>(users2);
        }



        public async Task<UserDto> GetAssignee(Target target)
        {
            string dep = RoleName.Admin;
            if (target == Target.Other)
            {
                var users = (await appUserRepository.WithDetailsAsync(d => d.Roles)).Where(d => d.Roles != null && d.Roles.Count > 0).Select(d => d.Id).ToArray();
                return await GetUserForTask(users);
            }

            switch (target)
            {
                case Target.BuyCommodity:
                    dep = RoleName.Buy;
                    break;
                case Target.BuyOther:
                    dep = RoleName.Admin;
                    break;
                case Target.Make:
                case Target.Storage:
                    dep = RoleName.Storage;
                    break;
                case Target.Payment:
                    dep = RoleName.Account;
                    break;

            }
            var users2 = await identityUserManager.GetUsersInRoleAsync(dep);
            return await GetUserForTask(users2.Select(c => c.Id).ToArray());
        }


        async Task<UserDto> GetUserForTask(Guid[] users)
        {
            if (users == null || users.Length == 0)
            {
                throw new BusinessException("400");
            }
            var tasks = taskRepository.Where(d => users.Contains(d.Assignee)).ToList().GroupBy(d => d.Assignee).OrderBy(d => d.Count());
            Guid userId;
            if (tasks.Count() == users.Length)
            {
                userId = tasks.FirstOrDefault().Key;
            }
            else
            {
                userId = users.Except(tasks.Select(d => d.Key)).FirstOrDefault();
            }

            var user = await appUserRepository.GetAsync(userId);
            return ObjectMapper.Map<IdentityUser, UserDto>(user);
        }

        public Task<TaskActionDto[]> GetTaskHistory(TaskHistoryRequestDto request)
        {
            var tasks = taskActionRepository
                .Where(d => d.TaskId == request.TaskId)
                .Skip(request.SkipCount)
                .Take(request.MaxResultCount)
                .ToList();
            var actions = tasks.Select(d => ObjectMapper.Map<TaskAction, TaskActionDto>(d)).ToArray();
            return STask.FromResult(actions);
        }

        public async Task<bool> ProcessTask(ProcessTaskRequest request)
        {
            var task = await taskRepository.GetAsync(d => d.Id == request.Id);
            if (task.Status != Status.Pending)
            {
                return false;
            }

            task.Status = request.Approved ? Status.Approved : Status.Rejected;

            var updated = await taskRepository.UpdateAsync(task);
            await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.Id, ActorId = CurrentUser.Id, Action = request.Approved ? ActionType.ApprovedTask : ActionType.RejectedTask, Note = request.Note });
            return true;
        }

        public async Task<bool> ReopenTask(ReopenTaskRequest request)
        {
            var task = await taskRepository.GetAsync(d => d.Id == request.Id);
            if (task.Status != Status.Completed || task.Status != Status.Incompleted)
            {
                return false;
            }

            task.Status = Status.Pending;
            var updated = await taskRepository.UpdateAsync(task);
            await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.Id, ActorId = CurrentUser.Id, Action = ActionType.Reopen });
            return true;
        }

        public async Task<bool> SendComment(CommentRequestDto request)
        {
            var result = await taskCommentRepository.InsertAsync(ObjectMapper.Map<CommentRequestDto, TaskComment>(request));
            if (result != null)
            {
                await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.TaskId, ActorId = CurrentUser.Id, Action = ActionType.Comment });
            }
            return true;
        }

        public async Task<MyTaskDto[]> GetMyActions()
        {
            bool isAdmin = false;
            var uid = CurrentUser.Id.Value;
            var user = await appUserRepository.GetAsync(uid);
            var roles = await identityUserManager.GetRolesAsync(user);
            isAdmin = roles.Any(c => c == RoleName.Admin);
            /// user : - create, assign, follow
            /// 
            /// admin : pending
            /// 

            var createdTasks = (await taskRepository.WithDetailsAsync(d => d.TaskHistory)).Where(d => d.CreatorId == uid && d.LastHistory != null).TakeLast(20).ToArray();
            var assignedTasks = (await taskRepository.WithDetailsAsync(d => d.TaskHistory)).Where(d => d.Assignee == uid && d.LastHistory != null).OrderByDescending(c => c.LastHistory.CreationTime).Take(20).ToArray();
            var followTasks = (await taskRepository.WithDetailsAsync(d => d.TaskHistory, c => c.TaskFollows)).Where(d => d.TaskFollows.Any(c => c.UserId == uid) && d.LastHistory != null).TakeLast(20).ToArray();
            if (isAdmin)
            {
                var pending = (await taskRepository.WithDetailsAsync(d => d.TaskHistory)).Where(d => d.Status == Status.Pending).TakeLast(20).ToArray();
                var adminTasks = createdTasks.Concat(assignedTasks).Concat(followTasks).Concat(pending).OrderByDescending(d => d.LastHistory.CreationTime).Take(20).ToArray();
                return ObjectMapper.Map<Task[], MyTaskDto[]>(adminTasks);
            }

            var tasks = createdTasks.Concat(assignedTasks).Concat(followTasks).OrderByDescending(d => d.LastHistory.CreationTime).Take(20).ToArray();
            List<System.Threading.Tasks.Task> taskGetUser = new List<System.Threading.Tasks.Task>();
            for (int i = 0; i < tasks.Length; i++)
            {
                var task = tasks[i];
                if (task.LastHistory?.ActorId.HasValue == true)
                {
                    taskGetUser.Add(appUserRepository.GetAsync(task.LastHistory.ActorId.Value).ContinueWith((d, obj) => obj = d.Result, task.LastHistory.Actor));
                }

            }
            await System.Threading.Tasks.Task.WhenAll(taskGetUser);
            return ObjectMapper.Map<Task[], MyTaskDto[]>(tasks);
        }

        public async Task<MyTaskDto[]> GetMyTasks(PagedResultRequestDto request)
        {
            //bool isAdmin = false;
            var uid = CurrentUser.Id.Value;
            //var user = await appUserRepository.GetAsync(uid);
            //var roles = await identityUserManager.GetRolesAsync(user);
            //isAdmin = roles.Any(c => c == RoleName.Admin);
            /// user : - create, assign, follow
            /// 
            /// admin : pending
            /// 

            var createdTasks = (await taskRepository.WithDetailsAsync(d => d.TaskHistory)).Where(d => d.CreatorId == uid).ToArray().ToArray();
            var assignedTasks = (await taskRepository.WithDetailsAsync(d => d.TaskHistory)).Where(d => d.Assignee == uid).ToArray();
            var followTasks = (await taskRepository.WithDetailsAsync(d => d.TaskHistory, c => c.TaskFollows)).Where(d => d.TaskFollows.Any(c => c.UserId == uid)).ToArray();
            //if (isAdmin)
            //{
            //    var pending = (await taskRepository.WithDetailsAsync(d => d.TaskHistory)).Where(d => d.Status == Status.Pending).TakeLast(20).ToArray();
            //    var adminTasks = createdTasks.Concat(assignedTasks).Concat(followTasks).Concat(pending).OrderByDescending(d => d.LastHistory.CreationTime).Take(20).ToArray();
            //    return ObjectMapper.Map<Task[], MyTaskDto[]>(adminTasks);
            //}

            var tasks = createdTasks.Concat(assignedTasks).Concat(followTasks).Where(d => d.LastHistory != null).OrderByDescending(d => d.LastHistory.CreationTime).Skip(request.SkipCount).Take(request.MaxResultCount).ToArray();
            List<System.Threading.Tasks.Task> taskGetUser = new List<System.Threading.Tasks.Task>();
            for (int i = 0; i < tasks.Length; i++)
            {
                var task = tasks[i];
                if (task.LastHistory?.ActorId.HasValue == true)
                {
                    taskGetUser.Add(appUserRepository.GetAsync(task.LastHistory.ActorId.Value).ContinueWith((d, obj) => obj = d.Result, task.LastHistory.Actor));
                }

            }
            await System.Threading.Tasks.Task.WhenAll(taskGetUser);
            return ObjectMapper.Map<Task[], MyTaskDto[]>(tasks);
        }
    }
}