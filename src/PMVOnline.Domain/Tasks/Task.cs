using PMVOnline.Targets;
using PMVOnline.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace PMVOnline.Tasks
{
    public class Task : FullAuditedAggregateRoot<long>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public Priority Priority { get; set; }
        public int TargetId { get; set; }
        public Status Status { get; set; }
        public ActionType LastAction { get; set; }
        public Guid AssigneeId { get; set; }
        public Guid? LeaderId { get; set; }



        [ForeignKey(nameof(AssigneeId))]
        public virtual AppUser Assignee { get; set; }

        [ForeignKey(nameof(TargetId))]
        public virtual Target Target { get; set; }

        [ForeignKey(nameof(CreatorId))]
        public virtual AppUser Creator { get; set; }
         
        [ForeignKey(nameof(LastModifierId))]
        public virtual AppUser LastModifier { get; set; }
         
        [ForeignKey(nameof(LeaderId))]
        public virtual AppUser Leader { get; set; }

        public virtual ICollection<TaskFollow> TaskFollows { get; set; }
        public virtual ICollection<TaskComment> TaskComments { get; set; }
        public virtual ICollection<TaskAction> TaskHistory { get; set; }
        public virtual ICollection<ReferenceTask> ReferenceTasks { get; set; }
        public virtual ICollection<TaskFile> TaskFiles { get; set; }
        public virtual ICollection<TaskRating> TaskRatings { get; set; }
    }
}
