using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace PMVOnline.Guides
{
    public class Guide : FullAuditedEntity<int>
    {
        public string Content { get; set; }
    }
}
