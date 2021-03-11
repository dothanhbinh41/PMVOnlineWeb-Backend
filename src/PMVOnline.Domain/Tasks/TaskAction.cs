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
        public long TaskId { get; set; } 
        public ActionType Action { get; set; }
        public string Note { get; set; } 

        [ForeignKey(nameof(TaskId))]
        public virtual Task Task { get; set; }

        [ForeignKey(nameof(CreatorId))]
        public virtual AppUser Actor { get; set; }
    } 
}
