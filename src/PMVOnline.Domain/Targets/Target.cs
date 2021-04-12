using PMVOnline.Departments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace PMVOnline.Targets
{
    public class Target : FullAuditedAggregateRoot<int>
    {
        public string Name { get; set; }
        public virtual ICollection<DepartmentTarget> DepartmentTargets { set; get; }
    }


    public class DepartmentTarget : FullAuditedAggregateRoot<int>
    {
        public long DepartmentId { get; set; }
        public int TargetId { get; set; }

        [ForeignKey(nameof(TargetId))]
        public virtual Target Target { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; }
    }
}