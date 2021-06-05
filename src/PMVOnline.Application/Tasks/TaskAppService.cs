using Microsoft.AspNetCore.Authorization;
using PMVOnline.Departments;
using PMVOnline.Files;
using PMVOnline.Notifications;
using PMVOnline.Targets;
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
        readonly ITargetManager targetManager;
        readonly IRepository<TaskRating> taskRatings;

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
            INotificationSender notificationSender,
            ITargetManager targetManager,
            IRepository<TaskRating> taskRatings
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
            this.targetManager = targetManager;
            this.taskRatings = taskRatings;
        }

        public async Task<TaskDto> CreateTask(CreateTaskRequestDto request)
        {
            var uid = CurrentUser.GetId();
            var task = ObjectMapper.Map<CreateTaskRequestDto, Task>(request);
            task.DueDate = task.DueDate.ToUniversalTime();
            var assignee = await appUserRepository.GetAsync(request.AssigneeId);
            task.LastModifierId = uid;

            var departments = await targetManager.GetDepartmentsByTargetAsync(request.TargetId);

            var users = await departmentManager.GetAllUserInDepartmentAsync(departments.Select(d => d.Id).ToArray());
            var leader = users.FirstOrDefault(d => d.IsLeader == true);
            if (leader != null)
            {
                task.LeaderId = leader.UserId;
            }
            var result = await taskRepository.InsertAsync(task, true);

            if (result == null)
            {
                throw new Exception();
            }

            await taskActionRepository.InsertAsync(new TaskAction { TaskId = task.Id, Action = ActionType.CreateTask });
            await notificationSender.SendNotifications(request.AssigneeId, $"Bạn được giao 1 sự vụ mới có Id là #{result.Id}");

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

            if (CurrentUser.GetId() != task.AssigneeId)
            {
                throw new UserFriendlyException("Ban khong the hoan thanh su vu");
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

        public async Task<UserDto[]> GetAllMember(int target)
        {
            var deps = await targetManager.GetDepartmentsByTargetAsync(target);
            var users = (await departmentManager.GetAllUserInDepartmentAsync(deps.Select(d => d.Id).ToArray())).Where(d => !d.IsLeader).Select(d => d.User).ToArray();
            return ObjectMapper.Map<AppUser[], UserDto[]>(users);
        }



        public async Task<UserDto> GetAssignee(int target)
        {
            var deps = await targetManager.GetDepartmentsByTargetAsync(target);
            var users = (await departmentManager.GetAllUserInDepartmentAsync(deps.Select(d => d.Id).ToArray())).Where(d => !d.IsLeader).Select(d => d.User).ToArray();
            return await GetUserForTask(users.Select(c => c.Id).ToArray());
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
            if (task.Status != Status.Requested)
            {
                return false;
            }

            var uid = CurrentUser.GetId();
            var departments = await departmentManager.GetUserDepartmentsAsync(uid);
            if (departments == null || !departments.Any(deparment => deparment?.Department?.Name == DepartmentName.Director))
            {
                throw new UserFriendlyException("Ban khong co quyen");
            }


            task.Status = request.Approved ? Status.Approved : Status.Rejected;
            task.LastAction = request.Approved ? ActionType.ApprovedTask : ActionType.RejectedTask;

            var updated = await taskRepository.UpdateAsync(task);
            await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.Id, Action = request.Approved ? ActionType.ApprovedTask : ActionType.RejectedTask, Note = request.Note });
            var followedUsers = taskFollowRepostiory.Where(d => d.TaskId == task.Id && d.Followed).ToArray().Select(d => d.UserId).Concat(new Guid[] { task.AssigneeId, uid }).Distinct().ToArray();
            await notificationSender.SendNotifications(followedUsers, request.Approved ? $"Sự vụ #{task.Id} đã được duyệt" : $"Sự vụ #{task.Id} không được duyệt");
            return true;
        }

        public async Task<bool> ReopenTask(ReopenTaskRequest request)
        {
            var uid = CurrentUser.GetId();
            var task = await taskRepository.GetAsync(d => d.Id == request.Id);
            if (task.Status != Status.Completed || task.Status != Status.Incompleted)
            {
                return false;
            }

            task.Status = Status.Pending;
            task.LastAction = ActionType.Reopen;
            var updated = await taskRepository.UpdateAsync(task);

            await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.Id, Action = ActionType.Reopen });
            var directors = (await departmentManager.GetAllUserAsync(DepartmentName.Director))?.ToArray()?.Select(d => d.UserId) ?? new Guid[0];
            var followedUsers = taskFollowRepostiory.Where(d => d.TaskId == task.Id && d.Followed).ToArray().Select(d => d.UserId).Concat(new Guid[] { task.AssigneeId, task.CreatorId.Value }).Concat(directors).Where(d => d != uid).Distinct().ToArray();
            await notificationSender.SendNotifications(followedUsers, $"Sự vụ #{task.Id} đã được mở lại");
            return true;
        }

        public async Task<bool> SendComment(long id, CommentRequestDto request)
        {
            var uid = CurrentUser.GetId();
            var task = await taskRepository.GetAsync(id);

            var comment = new TaskComment { TaskId = id, Comment = request.Comment, UserId = uid };
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
            var followedUsers = taskFollowRepostiory.Where(d => d.TaskId == task.Id && d.Followed).Select(d => d.UserId).ToArray().Concat(new Guid[] { task.AssigneeId }).Distinct().Where(d => d != uid).ToArray();
            await notificationSender.SendNotifications(followedUsers, $"Có bình luận cho sự vụ #{task.Id}");
            return true;
        }

        public async Task<MyTaskDto[]> GetMyActions()
        {
            var uid = CurrentUser.GetId();
            var departments = await departmentManager.GetUserDepartmentsAsync(uid);
            if (departments == null)
            {
                throw new UserFriendlyException("khong ton tai phong ban");
            }

            if (departments.Any(deparment => deparment?.Department?.Name == DepartmentName.Director))
            {
                var allTasks = (await taskRepository.WithDetailsAsync(d => d.LastModifier, d => d.Assignee, d => d.Creator, d => d.TaskFollows))
                    .Where(d => d.Status < Status.Rated && (d.Status == Status.Requested || d.TaskFollows.Any(c => c.UserId == uid && c.Followed) || d.CreatorId == uid || d.AssigneeId == uid))
                    .OrderByDescending(d => d.LastModificationTime)
                    .ToArray();
                return ObjectMapper.Map<Task[], MyTaskDto[]>(allTasks);
            }

            var userTasks = (await taskRepository.WithDetailsAsync(d => d.LastModifier, d => d.Assignee, d => d.Creator, d => d.TaskFollows))
                   .Where(d => d.Status != Status.Done &&
                   (d.TaskFollows.Any(c => c.UserId == uid && c.Followed) || (d.CreatorId == uid && d.Status != Status.Pending && d.Status != Status.Incompleted && d.Status != Status.LeaderRated) || (d.AssigneeId == uid && d.Status != Status.Completed && d.Status != Status.Incompleted && d.Status != Status.LeaderRated && d.Status != Status.Rated) || (d.LeaderId == uid && (d.Status == Status.Completed || d.Status == Status.Rated))))
                   .OrderByDescending(d => d.LastModificationTime)
                   .ToArray();
            return ObjectMapper.Map<Task[], MyTaskDto[]>(userTasks);
        }

        public async Task<MyTaskDto[]> SearchMyTasks(SearchMyTaskRequestDto request)
        {
            var uid = CurrentUser.GetId();

            var departments = await departmentManager.GetUserDepartmentsAsync(uid);
            if (departments == null)
            {
                throw new UserFriendlyException("khong ton tai phong ban");
            }

            if (departments.Any(deparment => deparment?.Department?.Name == DepartmentName.Director))
            {
                var allTask = (await taskRepository.WithDetailsAsync(d => d.LastModifier, d => d.Assignee, d => d.Creator))
                    .WhereIf(request.StartDate.HasValue, d => d.CreationTime.Date >= request.StartDate.Value.Date)
                    .WhereIf(request.EndDate.HasValue, d => d.CreationTime.Date <= request.EndDate.Value.Date)
                    .WhereIf(request.Users != null && request.Users.Length > 0, d => request.Users.Contains(d.AssigneeId) || request.Users.Contains(d.CreatorId.Value))
                    .OrderByDescending(d => d.Id)
                    .Skip(request.SkipCount)
                    .Take(request.MaxResultCount)
                    .ToArray();
                return ObjectMapper.Map<Task[], MyTaskDto[]>(allTask);
            }

            var departmentLeaders = departments.Where(d => d.IsLeader).ToArray();

            if (departmentLeaders.Length > 0)
            {
                var usersInDeparment = (await departmentManager.GetAllUserInDepartmentAsync(departmentLeaders.Select(c => c.DepartmentId).ToArray())).Select(d => d.UserId).ToArray();
                var departmentTasks = (await taskRepository.WithDetailsAsync(d => d.LastModifier, d => d.Assignee, d => d.Creator, d => d.TaskFollows))
                    .Where(d =>
                        usersInDeparment.Contains(d.AssigneeId) ||
                        d.TaskFollows.Any(c => c.UserId == uid && c.Followed) ||
                        d.CreatorId == uid)
                    .WhereIf(request.StartDate.HasValue, d => d.CreationTime.Date >= request.StartDate.Value.Date)
                    .WhereIf(request.EndDate.HasValue, d => d.CreationTime.Date <= request.EndDate.Value.Date)
                    .WhereIf(request.Users != null && request.Users.Length > 0, d => request.Users.Contains(d.AssigneeId) || request.Users.Contains(d.CreatorId.Value))
                    .OrderByDescending(d => d.Id)
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
                   .WhereIf(request.StartDate.HasValue, d => d.CreationTime.Date >= request.StartDate.Value.Date)
                   .WhereIf(request.EndDate.HasValue, d => d.CreationTime.Date <= request.EndDate.Value.Date)
                   .WhereIf(request.Users != null && request.Users.Length > 0, d => request.Users.Contains(d.AssigneeId) || request.Users.Contains(d.CreatorId.Value))
                   .OrderByDescending(d => d.Id)
                   .Skip(request.SkipCount)
                   .Take(request.MaxResultCount)
                   .ToArray();
            return ObjectMapper.Map<Task[], MyTaskDto[]>(userTasks);
        }

        public async Task<FullTaskDto> GetTask(long id)
        {
            var task = (await taskRepository.WithDetailsAsync(d => d.Assignee, d => d.Target)).FirstOrDefault(d => d.Id == id);
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

        async Task<Guid[]> GetAdminUserAsync()
        {
            var users = await departmentManager.GetAllUserAsync(DepartmentName.Director);
            return users.Select(d => d.UserId).ToArray();
        }

        public async Task<bool> RequestTask(RequestTaskRequest request)
        {
            var task = await taskRepository.GetAsync(request.Id);
            if (task.Status != Status.Pending)
            {
                return false;
            }

            var uid = CurrentUser.GetId();
            if (uid != task.CreatorId)
            {
                throw new UserFriendlyException("Ban khong co quyen");
            }

            task.Status = Status.Requested;
            task.LastAction = ActionType.RequestTask;
            var updated = await taskRepository.UpdateAsync(task);

            await taskActionRepository.InsertAsync(new TaskAction { TaskId = request.Id, Action = ActionType.RequestTask });


            var adminUsers = await GetAdminUserAsync();
            var followedUsers = taskFollowRepostiory.Where(d => d.TaskId == task.Id && d.Followed).ToArray().Select(d => d.UserId).ToArray().Concat(new Guid[] { task.AssigneeId }).Distinct().Where(d => d != uid && !adminUsers.Contains(d)).ToArray();

            await notificationSender.SendNotifications(adminUsers, $"Sự vụ #{task.Id} cần được duyệt");
            await notificationSender.SendNotifications(followedUsers, $"Sự vụ #{task.Id} đã được gửi yêu cầu duyệt");
            return true;
        }

        public async Task<SimpleUserDto[]> GetUsersInMyTasks()
        {
            var uid = CurrentUser.GetId();

            var departments = await departmentManager.GetUserDepartmentsAsync(uid);
            if (departments == null)
            {
                throw new UserFriendlyException("khong ton tai phong ban");
            }

            if (departments.Any(deparment => deparment?.Department?.Name == DepartmentName.Director))
            {
                return ObjectMapper.Map<AppUser[], SimpleUserDto[]>((await departmentManager.GetAllUserAsync()).ToArray().Select(d => d.User).Distinct().ToArray());
            }

            var departmentLeaders = departments.Where(d => d.IsLeader).ToArray();

            if (departmentLeaders.Length > 0)
            {
                var usersInDeparment = (await departmentManager.GetAllUserInDepartmentAsync(departmentLeaders.Select(c => c.DepartmentId).ToArray())).Select(d => d.User).Distinct().ToArray();
                return ObjectMapper.Map<AppUser[], SimpleUserDto[]>(usersInDeparment);
            }

            var users = (await taskRepository.WithDetailsAsync(d => d.Assignee, d => d.Creator)).Where(d => d.Assignee != null && d.Creator != null).ToArray().Select(d => new AppUser[] { d.Assignee, d.Creator }).SelectMany(d => d).Where(d => d.Id != uid).Distinct().ToArray();
            return ObjectMapper.Map<AppUser[], SimpleUserDto[]>(users);
        }

        public async Task<MyTaskDto[]> GetReferenceTasks(long id)
        {
            var tasks = referenceTaskRepostiory.Where(d => d.TaskId == id).ToArray().Select(d => d.ReferenceTaskId).ToArray();
            var rtasks = (await taskRepository.WithDetailsAsync(d => d.Assignee, d => d.Creator, d => d.LastModifier)).Where(d => tasks.Contains(d.Id)).ToArray();
            return ObjectMapper.Map<Task[], MyTaskDto[]>(rtasks);
        }

        public async Task<TaskDto> UpdateTask(UpdateTaskRequestDto request)
        {
            var uid = CurrentUser.GetId();
            var task = (await taskRepository.WithDetailsAsync(d => d.ReferenceTasks, d => d.TaskFiles)).FirstOrDefault(d => d.Id == request.Id);
            if (task == null || task.CreatorId != uid)
            {
                throw new Exception();
            }

            if (task.DueDate != request.DueDate.ToUniversalTime())
            {
                task.DueDate = request.DueDate.ToUniversalTime();
            }
            if (task.AssigneeId != request.AssigneeId)
            {
                task.AssigneeId = request.AssigneeId;
            }
            if (task.Content != request.Content)
            {
                task.Content = request.Content;
            }
            if (task.Title != request.Title)
            {
                task.Title = request.Title;
            }
            if (task.Priority != request.Priority)
            {
                task.Priority = request.Priority;
            }

            var result = await taskRepository.UpdateAsync(task, true);

            var refe = task.ReferenceTasks.ToArray().Select(d => d.ReferenceTaskId).ToArray();
            var removeTasks = refe.Except(request.ReferenceTasks ?? new long[0]).ToArray();
            var insertTasks = (request.ReferenceTasks ?? new long[0]).Except(refe).ToArray();

            if (removeTasks.Length > 0)
            {
                var rmTaskFiles = referenceTaskRepostiory.Where(d => d.TaskId == task.Id && removeTasks.Contains(d.ReferenceTaskId)).ToArray();
                await referenceTaskRepostiory.DeleteManyAsync(rmTaskFiles);
            }

            if (insertTasks.Length > 0)
            {
                await referenceTaskRepostiory.InsertManyAsync(insertTasks.Select(d => new ReferenceTask { TaskId = task.Id, ReferenceTaskId = d }));
            }

            var assignee = await appUserRepository.GetAsync(request.AssigneeId);

            var taskFiles = task.TaskFiles.ToArray().Select(d => d.FileId).ToArray();
            var removeFiles = taskFiles.Except(request.Files ?? new Guid[0]).ToArray();
            var insertFiles = (request.Files ?? new Guid[0]).Except(taskFiles).ToArray();

            if (removeFiles.Length > 0)
            {
                var rmTaskFiles = taskFileRepostiory.Where(d => d.TaskId == task.Id && removeFiles.Contains(d.FileId)).ToArray();
                await taskFileRepostiory.DeleteManyAsync(rmTaskFiles);
            }

            if (insertFiles.Length > 0)
            {
                await taskFileRepostiory.InsertManyAsync(insertFiles.Select(d => new TaskFile { TaskId = task.Id, FileId = d }));
            }


            return ObjectMapper.Map<Task, TaskDto>(result);
        }

        public async Task<bool> RateTaskAsync(long taskId, RatingRequestDto request)
        {
            var task = await taskRepository.GetAsync(taskId);
            var assignee = task.CreatorId;
            if (assignee == CurrentUser.GetId())
            {
                await taskRatings.InsertAsync(new TaskRating { TaskId = taskId, Rating = request.Rating, IsLeader = false });
                await CheckFinish(taskId);
                return true;
            }
            var departments = await targetManager.GetDepartmentsByTargetAsync(task.TargetId);

            var users = await departmentManager.GetAllUserInDepartmentAsync(departments.Select(d => d.Id).ToArray());
            var leader = users.FirstOrDefault(d => d.UserId == CurrentUser.GetId() && d.IsLeader == true);
            if (leader == null)
            {
                throw new UserFriendlyException("Ban khong phai truong phong de rating");
            }
            await taskRatings.InsertAsync(new TaskRating { TaskId = taskId, Rating = request.Rating, IsLeader = true });
            await CheckFinish(taskId);
            return true;
        }

        async Task<bool> CheckFinish(long taskId)
        {
            var count = taskRatings.Count(d => d.TaskId == taskId);
            if (count <= 1)
            {
                return false;
            }

            var task = await taskRepository.GetAsync(taskId);
            task.Status = Status.Done;
            await taskRepository.UpdateAsync(task);
            return true;
        }
    }
}