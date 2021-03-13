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
        public string FileName { get; set; } 
        public string FilePath { get; set; }


        [ForeignKey(nameof(CommentId))]
        public TaskComment Comment { get; set; }
    }
}