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
    public class TaskAction : FullAuditedAggregateRoot<Guid>
    {
        public ulong TaskId { get; set; }
        public Guid? ActorId { get; set; }
        public ActionType Action { get; set; }
        public string Note { get; set; }

        //public virtual IdentityUser Actor { get; set; }

        [ForeignKey(nameof(TaskId))]
        public virtual Task Task { get; set; }
        [NotMapped]
        public virtual IdentityUser Actor { get; set; }
    } 
}
