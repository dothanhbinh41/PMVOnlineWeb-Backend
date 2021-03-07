using System;
using System.Collections.Generic;
using System.Text;

namespace PMVOnline.Tasks
{
    public enum HistoryType
    {
        CreateTask, ApprovedTask, RejectedTask, Comment, CompletedTask, IncompletedTask, ChangeAssignee, Reopen, Follow, Unfollow
    }

    public enum NotificationType
    {
        RequestTask, AssignTask, ApprovedTask, RejectedTask, Comment, CompletedTask, IncompletedTask
    }
}