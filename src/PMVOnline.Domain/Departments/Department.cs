using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace PMVOnline.Departments
{
    public class Department : FullAuditedAggregateRoot<int>
    {
        public string Name { get; set; } 
        public virtual ICollection<DepartmentUser> DepartmentUsers { get; set; }
    }
}
