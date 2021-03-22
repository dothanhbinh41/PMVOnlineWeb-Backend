using System;
using System.Collections.Generic;
using System.Text;

namespace PMVOnline.Tasks
{
    public enum ActionType
    {
        CreateTask, RequestTask, ApprovedTask, RejectedTask, Comment, CompletedTask, IncompletedTask, ChangeAssignee, Reopen, Follow, Unfollow
    }

    public enum NotificationType
    {
        CreateTask, RequestTask, AssignTask, ApprovedTask, RejectedTask, Comment, CompletedTask, IncompletedTask
    }
}