using PMVOnline.Files;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace PMVOnline.Tasks
{
    public class TaskCommentFile : FullAuditedAggregateRoot<Guid>
    {
        public Guid CommentId { get; set; } 
        public Guid FileId { get; set; }

        [ForeignKey(nameof(FileId))]
        public virtual File File { get; set; }

        [ForeignKey(nameof(CommentId))]
        public virtual TaskComment Comment { get; set; }
    }
}