using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace PMVOnline.Profiles
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IProfileAppService), typeof(ProfileAppService), typeof(PMVProfileAppService))]
    public class PMVProfileAppService : ProfileAppService
    {
        private readonly IRepository<Volo.Abp.Identity.IdentityRole, Guid> roleRepository;

        public PMVProfileAppService(IdentityUserManager userManager, IOptions<IdentityOptions> identityOptions, IRepository<Volo.Abp.Identity.IdentityRole, Guid> roleRepository) : base(userManager, identityOptions)
        {
            this.roleRepository = roleRepository;
        }

        public async override Task<ProfileDto> GetAsync()
        {
            var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());
            var roles = await UserManager.GetRolesAsync(currentUser);

            var fullRoles = roleRepository.Where(d => roles.Contains(d.Name)).ToList();
            var roleDtos = ObjectMapper.Map<List<Volo.Abp.Identity.IdentityRole>, List<IdentityRoleDto>>(fullRoles);
            var profile = ObjectMapper.Map<Volo.Abp.Identity.IdentityUser, FullProfileDto>(currentUser);
            profile.Roles = roleDtos;
            return profile;
        }
    }
}
