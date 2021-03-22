﻿using Microsoft.AspNetCore.Authorization;
using PMVOnline.Departments;
using PMVOnline.Files;
using PMVOnline.Notifications;
using PMVOnline.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
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
        readonly IRepository<ReferenceTask, Guid> referenceTaskRepostiory;
        readonly IRepository<IdentityUser, Guid> appUserRepository;
        readonly IDepartmentManager departmentManager;
        readonly IRepository<File, Guid> fileRepository;
        readonly INotificationSender notificationSender;

        public TaskAppService(
            IRepository<Task, long> taskRepository,
            IRepository<TaskAction, Guid> taskActionRepository,
            IRepository<TaskComment, Guid> taskCommentRepository,
            IRepository<TaskFollow, Guid> taskFollowRepostiory,
            IRepository<TaskFile, Guid> taskFileRepostiory,
            IRepository<ReferenceTask, Guid> referenceTaskRepostiory,
            IRepository<IdentityUser, Guid> appUserRepository,
            IDepartmentManager departmentManager,
            IRepository<File, Guid> fileRepository,
            INotificationSender notificationSender
            )
        {
            this.taskRepository = taskRepository;
            this.taskActionRepository = taskActionRepository;
            this.taskCommentRepository = taskCommentRepository;
            this.taskFollowRepostiory = taskFollowRepostiory;
            this.taskFileRepostiory = taskFileRepostiory;
            this.referenceTaskRepostiory = referenceTaskRepostiory;
            this.appUserRepository = appUserRepository;
            this.departmentManager = departmentManager;
            this.fileRepository = fileRepository;
            this.notificationSender = notificationSender;
        }

        public async Task<TaskDto> CreateTask(CreateTaskRequestDto request)
        {
            var task = ObjectMapper.Map<CreateTaskRequestDto, Task>(request);
            var assignee = await appUserRepository.GetAsync(request.AssigneeId);
            task.LastModifierId = CurrentUser.GetId();
            var result = await taskRepository.InsertAsync(task, true);

            if (result == null)
            {
                throw new Exception();
            }

            await taskActionRepository.InsertAsync(new TaskAction { TaskId = task.Id, Action = ActionType.CreateTask });
            await notificationSender.SendNotifications(request.AssigneeId, "Bạn được giao sự vụ");
            await notificationSender.SendNotifications(await GetAdminUserFollowTaskAsync(task.Id), "Có sự vụ cần duyệt");

            if (request.Files?.Length > 0)
            {
                await taskFileRepostiory.InsertManyAsync(request.Files.Select(d => new TaskFile { FileId = d, TaskId = result.Id }));
            }
            if (request.ReferenceTasks?.Length > 0)
            {
                await referenceTaskRepostiory.InsertManyAsync(request.ReferenceTasks.Select(d => new ReferenceTask { ReferenceTaskId = d, TaskId = result.Id }));
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
            string dep = DepartmentName.Director;
            if (target == Target.Other)
            {
                var users = (await departmentManager.GetAllUserAsync()).Select(d => d.User).ToArray();
                return ObjectMapper.Map<AppUser[], UserDto[]>(users);
            }

            switch (target)
            {
                case Target.BuyCommodity:
                    dep = DepartmentName.Buy;
                    break;
                case Target.BuyOther:
                    dep = DepartmentName.Director;
                    break;
                case Target.Make:
                case Target.Storage:
                    dep = DepartmentName.Stocker;
                    break;
                case Target.Payment:
                    dep = DepartmentName.Accountant;
                    break;

            }

            var users2 = (await departmentManager.GetAllUserAsync(dep)).Select(d => d.User).ToArray();
            return ObjectMapper.Map<AppUser[], UserDto[]>(users2);
        }



        public async Task<UserDto> GetAssignee(Target target)
        {
            string dep = DepartmentName.Director;
            if (target == Target.Other)
            {
                var users = (await departmentManager.GetAllUserAsync()).Select(d => d.UserId).ToArray();
                return await GetUserForTask(users);
            }

            switch (target)
            {
                case Target.BuyCommodity:
                    dep = DepartmentName.Buy;
                    break;
                case Target.BuyOther:
                    dep = DepartmentName.Director;
                    break;
                case Target.Make:
                case Target.Storage:
                    dep = DepartmentName.Stocker;
                    break;
                case Target.Payment:
                    dep = DepartmentName.Accountant;
                    break;

            }
            var users2 = (await departmentManager.GetAllUserAsync(dep));
            return await GetUserForTask(users2.Select(c => c.UserId).ToArray());
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

        public async Task<TaskActionDto[]> GetTaskHistory(long id)
        {
            var tasks = (await taskActionRepository
                .WithDetailsAsync(d => d.Actor))
                .Where(d => d.TaskId == id)
                .OrderByDescending(d => d.CreationTime)
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

            await notificationSender.SendNotifications(task.AssigneeId, "Bạn được giao sự vụ");
            await notificationSender.SendNotifications(task.CreatorId.Value, "Sự vụ được xử lý");
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

        public async Task<bool> SendComment(long id, CommentRequestDto request)
        {
            var task = await taskRepository.GetAsync(id);

            var comment = new TaskComment { TaskId = id, Comment = request.Comment, UserId = CurrentUser.GetId() };
            if (request.Files?.Length > 0)
            {
                var commentsFiles = fileRepository.Where(d => request.Files.Contains(d.Id)).ToArray().Select(d => new TaskCommentFile
                {
                    FileId = d.Id,
                    FileName = d.Name,
                    FilePath = d.Path

                });
                comment.FileIds = new List<TaskCommentFile>(commentsFiles);
            }

            var result = await taskCommentRepository.InsertAsync(comment);
            if (result != null)
            {
                task.LastAction = ActionType.Comment;
                await taskActionRepository.InsertAsync(new TaskAction { TaskId = id, Action = ActionType.Comment });
                await taskRepository.UpdateAsync(task);
            }

            await notificationSender.SendNotifications(new Guid[] { task.AssigneeId, task.CreatorId.Value }, "Có bình luận sự vụ");
            await notificationSender.SendNotifications(GetUserFollowTask(task.Id), "Có bình luận sự vụ");
            return true;
        }

        public async Task<MyTaskDto[]> GetMyActions()
        {
            var uid = CurrentUser.GetId();
            var department = await departmentManager.GetUserDepartmentAsync(uid);
            if (department == null)
            {
                throw new UserFriendlyException("khong ton tai phong ban");
            }

            if (department.Department.Name == DepartmentName.Director)
            {
                var allTasks = (await taskRepository.WithDetailsAsync(d => d.LastModifier, d => d.Assignee, d => d.Creator, d => d.TaskFollows))
                    .Where(d => d.Status != Status.Completed && (d.Status == Status.Requested || d.TaskFollows.Any(c => c.UserId == uid) || d.CreatorId == uid || d.AssigneeId == uid))
                    .OrderByDescending(d => d.LastModificationTime)
                    .ToArray();
                return ObjectMapper.Map<Task[], MyTaskDto[]>(allTasks);
            }

            var userTasks = (await taskRepository.WithDetailsAsync(d => d.LastModifier, d => d.Assignee, d => d.Creator, d => d.TaskFollows))
                   .Where(d => d.Status != Status.Completed &&
                   (d.TaskFollows.Any(c => c.UserId == uid) || (d.CreatorId == uid && d.Status != Status.Pending) || d.AssigneeId == uid))
                   .OrderByDescending(d => d.LastModificationTime)
                   .ToArray();
            return ObjectMapper.Map<Task[], MyTaskDto[]>(userTasks);
        }

        public async Task<MyTaskDto[]> GetMyTasks(PagedResultRequestDto request)
        {
            var uid = CurrentUser.GetId();

            var department = await departmentManager.GetUserDepartmentAsync(uid);
            if (department == null)
            {
                throw new UserFriendlyException("khong ton tai phong ban");
            }

            if (department.Department.Name == DepartmentName.Director)
            {
                var allTask = (await taskRepository.WithDetailsAsync(d => d.LastModifier, d => d.Assignee, d => d.Creator)).Skip(request.SkipCount).Take(request.MaxResultCount).ToArray();
                return ObjectMapper.Map<Task[], MyTaskDto[]>(allTask);
            }

            if (department.IsLeader)
            {
                var usersInDeparment = (await departmentManager.GetAllUserAsync(department.DepartmentId)).Select(d => d.UserId).ToArray();
                var departmentTasks = (await taskRepository.WithDetailsAsync(d => d.LastModifier, d => d.Assignee, d => d.Creator, d => d.TaskFollows))
                    .Where(d =>
                        usersInDeparment.Contains(d.AssigneeId) ||
                        d.TaskFollows.Any(c => c.UserId == uid && c.Followed) ||
                        d.CreatorId == uid)
                    .OrderByDescending(d => d.CreationTime)
                    .Skip(request.SkipCount)
                    .Take(request.MaxResultCount)
                    .ToArray();
                return ObjectMapper.Map<Task[], MyTaskDto[]>(departmentTasks);
            }

            var userTasks = (await taskRepository.WithDetailsAsync(d => d.LastModifier, d => d.Assignee, d => d.Creator, d => d.TaskFollows))
                   .Where(d =>
                       d.TaskFollows.Any(c => c.UserId == uid && c.Followed) ||
                       d.CreatorId == uid ||
                       d.AssigneeId == uid)
                   .OrderByDescending(d => d.CreationTime)
                   .Skip(request.SkipCount)
                   .Take(request.MaxResultCount)
                   .ToArray();
            return ObjectMapper.Map<Task[], MyTaskDto[]>(userTasks);

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
            var comments = (await taskCommentRepository.WithDetailsAsync(d => d.FileIds, d => d.User)).Where(d => d.TaskId == id).OrderByDescending(d => d.CreationTime).ToArray();
            return ObjectMapper.Map<TaskComment[], TaskCommentDto[]>(comments);
        }

        public async Task<FileDto[]> GetTaskFiles(long id)
        {
            var files = (await taskFileRepostiory.WithDetailsAsync(d => d.File)).Where(d => d.TaskId == id).ToArray();
            return ObjectMapper.Map<TaskFile[], FileDto[]>(files);
        }

        public async Task<string> GetNote(long id)
        {
            var ac = taskActionRepository.OrderBy(d => d.CreationTime).LastOrDefault(d => d.TaskId == id && (d.Action == ActionType.RejectedTask || d.Action == ActionType.IncompletedTask || d.Action == ActionType.CompletedTask));
            return ac?.Note;
        }


        Guid[] GetUserFollowTask(long taskId)
        {
            return taskFollowRepostiory.Where(d => d.TaskId == taskId && d.Followed).Select(d => d.UserId).ToArray();
        }

        async Task<Guid[]> GetAdminUserFollowTaskAsync(long taskId)
        {
            var users = await departmentManager.GetAllUserAsync(DepartmentName.Director);
            return users.Select(d => d.UserId).ToArray();
        }
    }
}