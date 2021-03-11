using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace PMVOnline.Users
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IProfileAppService), typeof(ProfileAppService), typeof(UserAppService))]
    public class UserAppService : IdentityUserAppService
    {
        readonly IRepository<AppUser, Guid> appUsersRepository;

        public UserAppService(
            IRepository<AppUser, Guid> appUsersRepository,
            IdentityUserManager userManager,
            IIdentityUserRepository userRepository,
            IIdentityRoleRepository roleRepository,
            IOptions<IdentityOptions> identityOptions) : base(userManager, userRepository, roleRepository, identityOptions)
        {
            this.appUsersRepository = appUsersRepository;
        }

        public override async Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto input)
        {
            var result = await base.CreateAsync(input);
            var app = ObjectMapper.Map<IdentityUserDto, AppUser>(result);
            await appUsersRepository.InsertAsync(app);
            return result;
        }

        public override async Task DeleteAsync(Guid id)
        {
            await base.DeleteAsync(id);
            await appUsersRepository.DeleteAsync(id); 
        }

        public override async Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto input)
        {
            var result = await base.UpdateAsync(id, input); 
            var app = ObjectMapper.Map<IdentityUserDto, AppUser>(result);
            await appUsersRepository.UpdateAsync(app);
            return result;
        }
    }
}
