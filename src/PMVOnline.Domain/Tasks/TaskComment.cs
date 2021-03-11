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
    public class TaskComment : FullAuditedAggregateRoot<Guid>
    {
        public long TaskId { get; set; }
        public string Comment { get; set; }
        public Guid UserId { get; set; }
        public virtual TaskCommentFile[] FileIds { get; set; }
    

        [ForeignKey(nameof(TaskId))]
        public virtual Task Task { get; set; }

        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }
    }
}