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
        public Department()
        {

        }
        public Department(int id):base(id)
        {

        }
        public string Name { get; set; } 
        public virtual ICollection<DepartmentUser> DepartmentUsers { get; set; }
    }
}
