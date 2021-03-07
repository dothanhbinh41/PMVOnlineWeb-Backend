using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace PMVOnline.Files
{
    public class File : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }
    }
}
