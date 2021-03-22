using PMVOnline.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace PMVOnline.Departments
{
    public class DepartmentUser : FullAuditedAggregateRoot<long>
    {
        public Guid UserId { get; set; }
        public bool IsLeader { get; set; }
        public int DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; } 
    }
}
