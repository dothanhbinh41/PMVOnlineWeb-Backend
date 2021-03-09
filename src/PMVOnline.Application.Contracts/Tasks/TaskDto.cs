using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace PMVOnline.Tasks
{
    public class CreateTaskRequestDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Target Target { get; set; }
        public Priority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public Guid[] Files { get; set; }
        public ulong[] ReferenceTasks { get; set; }
        public Guid Assignee { get; set; }
    }

    public class TaskHistoryRequestDto : PagedResultRequestDto
    {
        public ulong TaskId { get; set; }
    }

    public class CommentRequestDto
    {
        public ulong TaskId { get; set; }
        public string Comment { get; set; }
        public Guid[] Files { get; set; }
    }

    public class ProcessTaskRequest : EntityDto<ulong>
    {
        public bool Approved { get; set; }
        public string Note { get; set; }
    }

    public class FinishTaskRequest : EntityDto<ulong>
    {
        public bool Completed { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Note { get; set; }
    }

    public class ReopenTaskRequest : EntityDto<ulong>
    {

    }

    public class FollowTaskRequest : EntityDto<ulong>
    {
        public bool Follow { get; set; }
    }

    public class UserDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public ICollection<RoleDto> Roles { set; get; }
    }

    public class RoleDto
    {
        public Guid RoleId { get; set; }
    }

    public class TaskDto : EntityDto<ulong>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public Priority Priority { get; set; }
        public Target Target { get; set; }
        public Status Status { get; set; }
        public Guid Assignee { get; set; }
    }

    public class TaskCommentDto
    {
    }

    public class TaskActionDto
    {
    }

    public class LastTaskHistoryDto
    {
        public Guid ActorId { get; set; }
        public UserDto Actor { get; set; }
        public ActionType Action { get; set; }

    }

    public class MyTaskDto
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; } 
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public Priority Priority { get; set; }
        public Target Target { get; set; }
        public Status Status { get; set; }
        public Guid Assignee { get; set; }
        public LastTaskHistoryDto LastAction { get; set; }
    } 
}