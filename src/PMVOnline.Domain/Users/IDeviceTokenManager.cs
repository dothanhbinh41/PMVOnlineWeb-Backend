using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace PMVOnline.Users
{
    public interface IDeviceTokenManager
    {
        Task<UserDeviceToken[]> GetUserDevicesAsync(Guid id);
        Task<UserDeviceToken[]> GetUsersDevicesAsync(Guid[] ids);
        Task<bool> DeleteToken(string[] tokens);
    }

    public class DeviceTokenManager : IDomainService, IDeviceTokenManager
    {

        readonly IRepository<UserDeviceToken> repository;

        public DeviceTokenManager(IRepository<UserDeviceToken> repository)
        {
            this.repository = repository;
        }

        public async Task<bool> DeleteToken(string[] tokens)
        {
            var tk = repository.Where(d => tokens.Contains(d.Token)).ToArray();
            await repository.DeleteManyAsync(tk);
            return true;
        }

        public Task<UserDeviceToken[]> GetUserDevicesAsync(Guid id)
        {
            return Task.FromResult(repository.Where(d => d.UserId == id).ToArray());
        }

        public Task<UserDeviceToken[]> GetUsersDevicesAsync(Guid[] ids)
        {
            return Task.FromResult(repository.Where(d => ids.Contains(d.UserId)).ToArray());
        }
    }
}
