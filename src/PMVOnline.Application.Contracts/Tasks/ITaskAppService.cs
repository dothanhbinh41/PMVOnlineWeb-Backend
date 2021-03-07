using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMVOnline.Tasks
{
    public interface ITaskAppService
    {
        Task<TaskDto> CreateTask(CreateTaskRequestDto request);
        Task<TaskActionDto[]> GetTaskHistory(TaskHistoryRequestDto request);
        Task<bool> SendComment(CommentRequestDto request);
        Task<bool> ProcessTask(ProcessTaskRequest request);
        Task<bool> FinishTask(FinishTaskRequest request);
        Task<bool> ReopenTask(ReopenTaskRequest request);
        Task<bool> FollowTask(FollowTaskRequest request);
        Task<UserDto> GetAssignee(Target target);
        Task<UserDto[]> GetAllMember(Target target);
    }
}
