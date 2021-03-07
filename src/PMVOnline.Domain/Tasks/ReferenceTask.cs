using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace PMVOnline.Tasks
{
    public class ReferenceTask : FullAuditedAggregateRoot<Guid>
    { 
        public ulong TaskId { get; set; }
        public ulong ReferenceTaskId { get; set; }

        [ForeignKey(nameof(TaskId))] 
        public virtual Task Task { get; set; } 
    }
}
