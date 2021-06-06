using PMVOnline.Files;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace PMVOnline.Tasks
{
    public interface ITaskAppService
    {
        Task<TaskDto> CreateTask(CreateTaskRequestDto request);
        Task<TaskDto> UpdateTask(UpdateTaskRequestDto request);
        Task<TaskActionDto[]> GetTaskHistory(long id);
        Task<string> GetNote(long id);
        Task<bool> SendComment(long id, CommentRequestDto request);
        Task<bool> ProcessTask(ProcessTaskRequest request);
        Task<bool> DeleteTask(long id);
        Task<bool> RequestTask(RequestTaskRequest request);
        Task<bool> FinishTask(FinishTaskRequest request);
        Task<bool> ReopenTask(ReopenTaskRequest request);
        Task<bool> FollowTask(FollowTaskRequest request);
        Task<UserDto> GetAssignee(int target);
        Task<UserDto[]> GetAllMember(int target);
        Task<MyTaskDto[]> GetMyActions();
        Task<MyTaskDto[]> SearchMyTasks(SearchMyTaskRequestDto request);
        Task<MyTaskDto[]> GetReferenceTasks(long id);
        Task<FullTaskDto> GetTask(long id);
        Task<TaskCommentDto[]> GetTaskComments(long id);
        Task<FileDto[]> GetTaskFiles(long id);
        Task<SimpleUserDto[]> GetUsersInMyTasks();

        Task<bool> RateTaskAsync(long taskId, RatingRequestDto request);
    }
}
