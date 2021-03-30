using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace PMVOnline.Tasks
{
    public class ReferenceTask : FullAuditedAggregateRoot<Guid>
    { 
        public long TaskId { get; set; }
        public long ReferenceTaskId { get; set; }

        [ForeignKey(nameof(ReferenceTaskId))] 
        public virtual Task Task { get; set; } 
    }
}
