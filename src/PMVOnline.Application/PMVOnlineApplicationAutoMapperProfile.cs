using AutoMapper;
using PMVOnline.Files;
using PMVOnline.Profiles;
using PMVOnline.Tasks;
using PMVOnline.Users;
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
            CreateMap<IdentityUser, SimpleUserDto>();
            CreateMap<IdentityUser, UserDto>();
            CreateMap<IdentityUser, FullProfileDto>()
                .ForMember(dest => dest.HasPassword, op => op.MapFrom(src => src.PasswordHash != null))
                .MapExtraProperties();
            CreateMap<IdentityUserRole, RoleDto>();
            CreateMap<File, FileDto>();
            CreateMap<TaskAction, TaskActionDto>();
            CreateMap<Task, TaskDto>();
            CreateMap<IdentityUserDto, AppUser>();
            CreateMap<AppUser, SimpleUserDto>();
            CreateMap<Task, MyTaskDto>();
            //.ForMember=>d.)
            //.MapExtraProperties();
        }
    }
}
