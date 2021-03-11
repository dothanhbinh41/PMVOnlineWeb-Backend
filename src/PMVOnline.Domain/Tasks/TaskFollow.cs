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
    public class TaskFollow : FullAuditedAggregateRoot<Guid>
    {
        public long TaskId { get; set; }
        public Guid UserId { get; set; }
        public bool Followed { get; set; }


        [ForeignKey(nameof(TaskId))]
        public virtual Task Task { get; set; }

        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }
    }
}