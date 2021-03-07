using AutoMapper.Configuration.Annotations;
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
    public class TaskHistory : FullAuditedAggregateRoot<Guid>
    {
        public ulong TaskId { get; set; }
        public Guid? ActorId { get; set; }
        public HistoryType Action { get; set; }
        public string Note { get; set; }

        //public virtual IdentityUser Actor { get; set; }

        [ForeignKey(nameof(TaskId))]
        public virtual Task Task { get; set; }
    }

    public class TaskNotification : FullAuditedAggregateRoot<Guid>
    {
        public ulong TaskId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? ActorId { get; set; }
        public NotificationType NotificationType { get; set; } 

        //public virtual IdentityUser Actor { get; set; }

        [ForeignKey(nameof(TaskId))]
        public virtual Task Task { get; set; }
    }
}
