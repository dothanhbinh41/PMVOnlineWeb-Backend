﻿using System;
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
        public long[] ReferenceTasks { get; set; }
        public Guid Assignee { get; set; }
    }

    public class TaskHistoryRequestDto : PagedResultRequestDto
    {
        public long TaskId { get; set; }
    }

    public class CommentRequestDto
    {
        public long TaskId { get; set; }
        public string Comment { get; set; }
        public Guid[] Files { get; set; }
    }

    public class ProcessTaskRequest : EntityDto<long>
    {
        public bool Approved { get; set; }
        public string Note { get; set; }
    }

    public class FinishTaskRequest : EntityDto<long>
    {
        public bool Completed { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Note { get; set; }
    }

    public class ReopenTaskRequest : EntityDto<long>
    {

    }

    public class FollowTaskRequest : EntityDto<long>
    {
        public bool Follow { get; set; }
    }

    public class SimpleUserDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Surname { get; set; } 
    }

    public class UserDto : SimpleUserDto
    { 
        public ICollection<RoleDto> Roles { set; get; }
    }

    public class RoleDto
    {
        public Guid RoleId { get; set; }
    }

    public class TaskDto : EntityDto<long>
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
        public SimpleUserDto Actor { get; set; }
        public ActionType Action { get; set; }

    }

    public class MyTaskDto
    {
        public long TaskId { get; set; }
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