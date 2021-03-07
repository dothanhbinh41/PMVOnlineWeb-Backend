using AutoMapper;
using PMVOnline.Tasks;
using Volo.Abp.Identity;

namespace PMVOnline
{
    public class PMVOnlineApplicationAutoMapperProfile : Profile
    {
        public PMVOnlineApplicationAutoMapperProfile()
        {
            CreateMap<CreateTaskRequestDto, Task>()
                .ForMember(d => d.ReferenceTasks, c => c.Ignore())
                .ForMember(d=>d.TaskFiles,c=>c.Ignore());
            CreateMap<IdentityUser, UserDto>();
            CreateMap<IdentityUserRole, RoleDto>();
            //.ForMember(d=>d.)
            //.MapExtraProperties();
        }
    }
}
