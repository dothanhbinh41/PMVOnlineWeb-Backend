using AutoMapper;
using PMVOnline.Files;
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
                .ForMember(d => d.TaskFiles, c => c.Ignore());
            CreateMap<IdentityUser, UserDto>();
            CreateMap<IdentityUserRole, RoleDto>();
            CreateMap<File, FileDto>();
            CreateMap<TaskAction, LastTaskHistoryDto>();
            CreateMap<Task, MyActionDto>()
                .ForMember(d => d.LastAction, c => c.MapFrom(d => d.LastHistory))
                .ForMember(d => d.TaskId, c => c.MapFrom(d => d.Id));
            //.ForMember=>d.)
            //.MapExtraProperties();
        }
    }
}
