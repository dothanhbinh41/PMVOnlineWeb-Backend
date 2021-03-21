using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMVOnline.Users
{
    public interface IDeviceTokenAppService
    {
        Task<bool> SaveDeviceTokenAsync(SaveDeviceTokenDto dto);
    }
}
