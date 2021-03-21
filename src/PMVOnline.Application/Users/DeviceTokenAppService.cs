using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace PMVOnline.Users
{
    public class DeviceTokenAppService : ApplicationService, IDeviceTokenAppService
    {
        readonly IRepository<UserDeviceToken> repository;

        public DeviceTokenAppService(IRepository<UserDeviceToken> repository)
        {
            this.repository = repository;
        }

        public async Task<bool> SaveDeviceTokenAsync(SaveDeviceTokenDto dto)
        {
            var obj = ObjectMapper.Map<SaveDeviceTokenDto, UserDeviceToken>(dto);
            obj.UserId = CurrentUser.GetId();
            await repository.InsertAsync(obj);
            return true;
        }
    }
}
