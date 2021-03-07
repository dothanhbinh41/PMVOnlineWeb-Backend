using System;
using System.Collections.Generic;
using System.Text;

namespace PMVOnline.Tasks
{
    public enum Action
    {
        CreateTask, AssignTask, RequestTask, ApprovedTask, RejectedTask, Comment, CompletedTask, IncompletedTask, ChangeAssignee, Reopen, Follow, Unfollow
    }
}