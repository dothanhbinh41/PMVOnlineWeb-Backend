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
        public Target Target { get; set; }
        public Status Status { get; set; }
        public ActionType LastAction { get; set; }
        public Guid AssigneeId { get; set; }


        public virtual ICollection<TaskAction> TaskHistory { get; set; } 
        public virtual ICollection<ReferenceTask> ReferenceTasks { get; set; } 
        public virtual ICollection<TaskFile> TaskFiles { get; set; } 

        [ForeignKey(nameof(AssigneeId))]
        public virtual AppUser Assignee { get; set; }

        [ForeignKey(nameof(CreatorId))]
        public virtual AppUser Creator { get; set; }
         
        [ForeignKey(nameof(LastModifierId))]
        public virtual AppUser LastModifier { get; set; }
        public virtual ICollection<TaskFollow> TaskFollows { get; set; }
    }
}
