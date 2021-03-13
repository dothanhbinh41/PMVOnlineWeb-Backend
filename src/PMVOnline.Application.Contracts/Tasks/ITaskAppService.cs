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
        Task<TaskActionDto[]> GetTaskHistory(long id,TaskHistoryRequestDto request);
        Task<bool> SendComment(CommentRequestDto request);
        Task<bool> ProcessTask(ProcessTaskRequest request);
        Task<bool> FinishTask(FinishTaskRequest request);
        Task<bool> ReopenTask(ReopenTaskRequest request);
        Task<bool> FollowTask(FollowTaskRequest request);
        Task<UserDto> GetAssignee(Target target);
        Task<UserDto[]> GetAllMember(Target target);
        Task<MyTaskDto[]> GetMyActions();
        Task<MyTaskDto[]> GetMyTasks(PagedResultRequestDto request);
        Task<FullTaskDto> GetTask(long id);
        Task<TaskCommentDto[]> GetTaskComments(long id); 
        Task<FileDto[]> GetTaskFiles(long id); 
    }
}
