using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace PMVOnline.Tasks
{
    public class Task : FullAuditedAggregateRoot<ulong>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public Priority Priority { get; set; }
        public Target Target { get; set; }
        public Status Status { get; set; }
        public Guid Assignee { get; set; }

        public virtual ICollection<TaskFile> TaskFiles { get; set; }
        public virtual ICollection<ReferenceTask> ReferenceTasks { get; set; }
        public virtual ICollection<TaskAction> TaskHistory { get; set; }
         
        [NotMapped]
        public virtual TaskAction LastHistory => TaskHistory.LastOrDefault();
        public virtual ICollection<TaskFollow> TaskFollows { get; set; } 
    }
}
