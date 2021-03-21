using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace PMVOnline.Users
{
    public class UserDeviceToken : FullAuditedAggregateRoot<long>
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DeviceType Device { get; set; }
    }
}
