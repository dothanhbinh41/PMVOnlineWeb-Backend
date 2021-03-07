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

        public TaskAppService(
            IRepository<Task, ulong> taskRepository,
            IRepository<TaskAction, Guid> taskActionRepository,
            IRepository<TaskComment, Guid> taskCommentRepository,
            IRepository<TaskFollow, Guid> taskFollowRepostiory,
            IRepository<IdentityUser, Guid> appUserRepository,
            IRepository<IdentityRole, Guid> roleRepository
            )
        {
            this.taskRepository = taskRepository;
            this.taskActionRepository = taskActionRepository;
            this.taskCommentRepository = taskCommentRepository;
            this.taskFollowRepostiory = taskFollowRepostiory;
            this.appUserRepository = appUserRepository;
            this.roleRepository = roleRepository;
        }

        public async Task<TaskDto> CreateTask(CreateTaskRequestDto request)
        {
            var task = ObjectMapper.Map<CreateTaskRequestDto, Task>(request);

            var assignee = await appUserRepository.GetAsync(request.Assignee);
            var result = await taskRepository.InsertAsync(task);
            var id = CurrentUser.Id.Value;
            if (result != null)
            {
                var actions = new TaskAction[] {
                    new TaskAction { TaskId = task.Id, UserId = request.Assignee, ActorId = id, Action = Action.AssignTask },
                    new TaskAction { TaskId = task.Id, UserId =  id, ActorId = id, Action = Action.CreateTask }
                };
                await taskActionRepository.InsertManyAsync(actions);
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
            await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.Id, UserId = CurrentUser.Id.Value, ActorId = CurrentUser.Id, Action = request.Completed ? Action.CompletedTask : Action.IncompletedTask, Note = request.Note });
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
            await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.Id, UserId = CurrentUser.Id.Value, ActorId = CurrentUser.Id, Action = request.Follow ? Action.Follow : Action.Unfollow });
            return true;
        }

        public async Task<UserDto[]> GetAllMember(Target target)
        {
            string dep = RoleName.Admin;
            IdentityUser[] users = null;
            if (target == Target.Other)
            {
                users = (await appUserRepository.WithDetailsAsync(d => d.Roles)).Where(d => d.Roles != null && d.Roles.Count > 0).ToArray();
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

            if (users == null)
            {
                var role = await roleRepository.FirstOrDefaultAsync(d => d.Name == dep);
                if (role == null)
                {
                    throw new Exception();
                }
                users = (await appUserRepository.WithDetailsAsync(d => d.Roles)).Where(d => d.Roles != null && d.Roles.Any(c => c.RoleId == role.Id)).ToArray();
            } 
            return ObjectMapper.Map<IdentityUser[], UserDto[]>(users);
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
             
            var role = await roleRepository.FirstOrDefaultAsync(d => d.Name == dep);
            if (role == null)
            {
                throw new Exception();
            }
            var users2 = (await appUserRepository.WithDetailsAsync(d => d.Roles)).Where(d => d.Roles != null && d.Roles.Any(c => c.RoleId == role.Id)).Select(d => d.Id).ToArray(); 
            return await GetUserForTask(users2);
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
                .Where(d => d.UserId == CurrentUser.Id && d.TaskId == request.TaskId)

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
            await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.Id, UserId = CurrentUser.Id.Value, ActorId = CurrentUser.Id, Action = request.Approved ? Action.ApprovedTask : Action.RejectedTask, Note = request.Note });
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
            await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.Id, UserId = CurrentUser.Id.Value, ActorId = CurrentUser.Id, Action = Action.Reopen });
            return true;
        }

        public async Task<bool> SendComment(CommentRequestDto request)
        {
            var result = await taskCommentRepository.InsertAsync(ObjectMapper.Map<CommentRequestDto, TaskComment>(request));
            if (result != null)
            {
                await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.TaskId, UserId = CurrentUser.Id.Value, ActorId = CurrentUser.Id, Action = Action.Comment });
            }
            return true;
        }
    }
}