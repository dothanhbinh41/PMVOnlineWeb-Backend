using Microsoft.AspNetCore.Authorization;
using PMVOnline.Departments;
using PMVOnline.Files;
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
using Volo.Abp.Users;
using STask = System.Threading.Tasks.Task;

namespace PMVOnline.Tasks
{
    [Authorize]
    public class TaskAppService : ApplicationService, ITaskAppService
    {
        readonly IRepository<Task, long> taskRepository;
        readonly IRepository<TaskAction, Guid> taskActionRepository;
        readonly IRepository<TaskComment, Guid> taskCommentRepository;
        readonly IRepository<TaskFollow, Guid> taskFollowRepostiory;
        readonly IRepository<TaskFile, Guid> taskFileRepostiory;
        readonly IRepository<IdentityUser, Guid> appUserRepository;
        readonly IRepository<IdentityRole, Guid> roleRepository;
        readonly IRepository<File, Guid> fileRepository;
        readonly IdentityUserManager identityUserManager;

        public TaskAppService(
            IRepository<Task, long> taskRepository,
            IRepository<TaskAction, Guid> taskActionRepository,
            IRepository<TaskComment, Guid> taskCommentRepository,
            IRepository<TaskFollow, Guid> taskFollowRepostiory,
            IRepository<TaskFile, Guid> taskFileRepostiory,
            IRepository<IdentityUser, Guid> appUserRepository,
            IRepository<IdentityRole, Guid> roleRepository,
            IRepository<File, Guid> fileRepository,
            IdentityUserManager identityUserManager
            )
        {
            this.taskRepository = taskRepository;
            this.taskActionRepository = taskActionRepository;
            this.taskCommentRepository = taskCommentRepository;
            this.taskFollowRepostiory = taskFollowRepostiory;
            this.taskFileRepostiory = taskFileRepostiory;
            this.appUserRepository = appUserRepository;
            this.roleRepository = roleRepository;
            this.fileRepository = fileRepository;
            this.identityUserManager = identityUserManager;
        }

        public async Task<TaskDto> CreateTask(CreateTaskRequestDto request)
        {
            var task = ObjectMapper.Map<CreateTaskRequestDto, Task>(request);
            var assignee = await appUserRepository.GetAsync(request.AssigneeId);
            task.LastModifierId = CurrentUser.GetId();
            var result = await taskRepository.InsertAsync(task, true);
            if (result != null)
            {
                await taskActionRepository.InsertAsync(new TaskAction { TaskId = task.Id, Action = ActionType.CreateTask });
            }

            return ObjectMapper.Map<Task, TaskDto>(result);
        }

        public async Task<bool> FinishTask(FinishTaskRequest request)
        {
            var task = await taskRepository.GetAsync(request.Id);
            if (task.Status != Status.Approved)
            {
                return false;
            }

            task.Status = request.Completed ? Status.Completed : Status.Incompleted;
            task.LastAction = request.Completed ? ActionType.CompletedTask : ActionType.IncompletedTask;
            if (request.CompletedDate.HasValue && request.Completed)
            {
                task.CompletedDate = request.CompletedDate;
            }

            var updated = await taskRepository.UpdateAsync(task);
            await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.Id, Action = request.Completed ? ActionType.CompletedTask : ActionType.IncompletedTask, Note = request.Note });
            return true;
        }

        public async Task<bool> FollowTask(FollowTaskRequest request)
        {
            var taskFollow = await taskFollowRepostiory.FirstOrDefaultAsync(d => d.TaskId == request.Id && d.UserId == CurrentUser.Id.Value);
            if (taskFollow == null)
            {
                await taskFollowRepostiory.InsertAsync(new TaskFollow { TaskId = request.Id, UserId = CurrentUser.Id.Value, Followed = true });
            }
            else
            {
                taskFollow.Followed = request.Follow;
                await taskFollowRepostiory.UpdateAsync(taskFollow);
            }
            var task = await taskRepository.GetAsync(request.Id);
            task.LastAction = request.Follow ? ActionType.Follow : ActionType.Unfollow;
            await taskRepository.UpdateAsync(task);
            await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.Id, Action = request.Follow ? ActionType.Follow : ActionType.Unfollow });
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
            var tasks = taskRepository.Where(d => users.Contains(d.AssigneeId)).ToList().GroupBy(d => d.AssigneeId).OrderBy(d => d.Count());
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

        public async Task<TaskActionDto[]> GetTaskHistory(long id, TaskHistoryRequestDto request)
        {
            var tasks = (await taskActionRepository
                .WithDetailsAsync(d => d.Actor))
                .Where(d => d.TaskId == id)
                .Skip(request.SkipCount)
                .Take(request.MaxResultCount)
                .ToList();
            var actions = tasks.Select(d => ObjectMapper.Map<TaskAction, TaskActionDto>(d)).ToArray();
            return actions;
        }

        public async Task<bool> ProcessTask(ProcessTaskRequest request)
        {
            var task = await taskRepository.GetAsync(request.Id);
            if (task.Status != Status.Pending)
            {
                return false;
            }

            task.Status = request.Approved ? Status.Approved : Status.Rejected;
            task.LastAction = request.Approved ? ActionType.ApprovedTask : ActionType.RejectedTask;

            var updated = await taskRepository.UpdateAsync(task);
            await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.Id, Action = request.Approved ? ActionType.ApprovedTask : ActionType.RejectedTask, Note = request.Note });
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
            task.LastAction = ActionType.Reopen;
            var updated = await taskRepository.UpdateAsync(task);
            await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.Id, Action = ActionType.Reopen });
            return true;
        }

        public async Task<bool> SendComment(CommentRequestDto request)
        {
            var task = await taskRepository.GetAsync(request.TaskId);

            var comment = ObjectMapper.Map<CommentRequestDto, TaskComment>(request);
            var commentsFiles = fileRepository.Where(d => request.Files.Contains(d.Id)).ToArray().Select(d => new TaskCommentFile
            {
                CommentId = comment.Id,
                FileId = d.Id,
                FileName = d.Name,
                FilePath = d.Path

            });
            comment.FileIds = new List<TaskCommentFile>(commentsFiles);
            var result = await taskCommentRepository.InsertAsync(comment);
            if (result != null)
            {
                task.LastAction = ActionType.Comment;
                await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.TaskId, Action = ActionType.Comment });
                await taskRepository.UpdateAsync(task);
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

            var createdTasks = (await taskRepository.WithDetailsAsync(d => d.LastModifier, d => d.Assignee, d => d.Creator)).Where(d => d.CreatorId == uid).OrderByDescending(c => c.LastModificationTime.HasValue ? c.LastModificationTime.Value : c.CreationTime).Take(20).ToArray();
            var assignedTasks = (await taskRepository.WithDetailsAsync(d => d.LastModifier, d => d.Assignee, d => d.Creator)).Where(d => d.AssigneeId == uid).OrderByDescending(c => c.LastModificationTime.HasValue ? c.LastModificationTime.Value : c.CreationTime).Take(20).ToArray();
            var followTasks = (await taskRepository.WithDetailsAsync(d => d.LastModifier, c => c.TaskFollows, d => d.Assignee, d => d.Creator)).Where(d => d.TaskFollows.Any(c => c.UserId == uid)).OrderByDescending(c => c.LastModificationTime.HasValue ? c.LastModificationTime.Value : c.CreationTime).Take(20).ToArray();
            if (isAdmin)
            {
                var pending = (await taskRepository.WithDetailsAsync(d => d.LastModifier, d => d.Assignee, d => d.Creator)).Where(d => d.Status == Status.Pending).OrderByDescending(c => c.LastModificationTime.HasValue ? c.LastModificationTime.Value : c.CreationTime).Take(20).ToArray();
                var adminTasks = createdTasks.Concat(assignedTasks).Concat(followTasks).Concat(pending).OrderByDescending(c => c.LastModificationTime.HasValue ? c.LastModificationTime.Value : c.CreationTime).Take(20).ToArray();
                return ObjectMapper.Map<Task[], MyTaskDto[]>(adminTasks);
            }

            var tasks = createdTasks.Concat(assignedTasks).Concat(followTasks).OrderByDescending(c => c.LastModificationTime.HasValue ? c.LastModificationTime.Value : c.CreationTime).Distinct().Take(20).ToArray();

            return ObjectMapper.Map<Task[], MyTaskDto[]>(tasks);
        }

        public async Task<MyTaskDto[]> GetMyTasks(PagedResultRequestDto request)
        {
            var uid = CurrentUser.GetId();
            var createdTasks = (await taskRepository.WithDetailsAsync(d => d.LastModifier, d => d.Assignee, d => d.Creator)).Where(d => d.CreatorId == uid).ToArray().ToArray();
            var assignedTasks = (await taskRepository.WithDetailsAsync(d => d.LastModifier, d => d.Assignee, d => d.Creator)).Where(d => d.AssigneeId == uid).ToArray();
            var followTasks = (await taskRepository.WithDetailsAsync(d => d.LastModifier, c => c.TaskFollows, d => d.Assignee, d => d.Creator)).Where(d => d.TaskFollows.Any(c => c.UserId == uid)).ToArray();
            var tasks = createdTasks.Concat(assignedTasks).Concat(followTasks).OrderByDescending(d => d.CreationTime).Distinct().Skip(request.SkipCount).Take(request.MaxResultCount).ToArray();
            return ObjectMapper.Map<Task[], MyTaskDto[]>(tasks);
        }

        public async Task<FullTaskDto> GetTask(long id)
        {
            var task = (await taskRepository.WithDetailsAsync(d => d.Assignee, d => d.ReferenceTasks)).FirstOrDefault(d => d.Id == id);
            if (task == null)
            {
                return null;
            }

            return ObjectMapper.Map<Task, FullTaskDto>(task);
        }

        public async Task<TaskCommentDto[]> GetTaskComments(long id)
        {
            var comments = (await taskCommentRepository.WithDetailsAsync(d => d.FileIds)).Where(d => d.TaskId == id).ToArray();
            return ObjectMapper.Map<TaskComment[], TaskCommentDto[]>(comments);
        }

        public async Task<FileDto[]> GetTaskFiles(long id)
        {
            var files = (await taskFileRepostiory.WithDetailsAsync(d => d.File)).Where(d => d.TaskId == id).ToArray();
            return ObjectMapper.Map<TaskFile[], FileDto[]>(files);
        }
    }
}