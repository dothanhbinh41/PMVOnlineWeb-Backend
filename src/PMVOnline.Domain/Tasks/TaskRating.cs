using PMVOnline.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace PMVOnline.Tasks
{
    public class TaskRating : FullAuditedAggregateRoot<long>
    {
        public long TaskId { get; set; }
        public int Rating { get; set; }
        public bool IsLeader { get; set; }
        public string Note { get; set; }

        [ForeignKey(nameof(TaskId))]
        public virtual Task Task { get; set; }


        [ForeignKey(nameof(CreatorId))]
        public virtual AppUser Creator { get; set; }
    }
}
