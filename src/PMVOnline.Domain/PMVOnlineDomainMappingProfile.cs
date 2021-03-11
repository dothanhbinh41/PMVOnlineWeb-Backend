using AutoMapper;
using PMVOnline.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace PMVOnline
{
    public class PMVOnlineDomainMappingProfile : Profile
    {
        public PMVOnlineDomainMappingProfile()
        {
            CreateMap<IdentityUser, AppUser>();
        }
    }
}
