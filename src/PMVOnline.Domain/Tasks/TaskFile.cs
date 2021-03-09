using PMVOnline.Files;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace PMVOnline.Tasks
{
    public class TaskFile : FullAuditedAggregateRoot<Guid>
    {
        public long TaskId { get; set; }
        public Guid FileId { get; set; }

        [ForeignKey(nameof(FileId))]
        public virtual File File { get; set; }

        [ForeignKey(nameof(TaskId))]
        public virtual Task Task { get; set; }
    }
}
