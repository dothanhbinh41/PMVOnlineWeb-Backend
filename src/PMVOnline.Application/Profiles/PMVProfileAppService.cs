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

        public PMVProfileAppService(IdentityUserManager userManager, IOptions<IdentityOptions> identityOptions) : base(userManager, identityOptions)
        {
        }

        public async override Task<ProfileDto> GetAsync()
        {
            var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());
            var profile = ObjectMapper.Map<Volo.Abp.Identity.IdentityUser, FullProfileDto>(currentUser);
            return profile;
        }
    }
}
