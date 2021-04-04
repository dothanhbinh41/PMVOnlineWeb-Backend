using AutoMapper;
using PMVOnline.Departments;
using PMVOnline.Files;
using PMVOnline.Guides;
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
                .ForMember(d=>d.TaskFollows,d=>d.Ignore())
                .ForMember(d=>d.ReferenceTasks, d=>d.Ignore())
                .ForMember(d=>d.TaskHistory, d=>d.Ignore())
                .ForMember(d=>d.TaskFiles,d=>d.Ignore());
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
            CreateMap<TaskFile, TaskFileDto>();
            CreateMap<ReferenceTask, ReferenceTaskDto>();
            CreateMap<Task, FullTaskDto>();
            CreateMap<Guide, GuideDto>();
            CreateMap<TaskComment, TaskCommentDto>();
            CreateMap<TaskCommentFile, TaskFileDto>();
            CreateMap<IdentityRole, IdentityRoleDto>();
            CreateMap<CommentRequestDto, TaskComment>();
            CreateMap<TaskCommentFile, CommentFileDto>();
            CreateMap<SaveDeviceTokenDto, UserDeviceToken>();
            CreateMap<Department, DepartmentDto>();
            CreateMap<DepartmentUser, DepartmentUserDto>();
            CreateMap<UpdateDepartmentUserDto, DepartmentUser>();
            CreateMap<CreateDepartmentUserDto, DepartmentUser>();
            CreateMap<DeleteDepartmentUserDto, DepartmentUser>();
            CreateMap<TaskFile, FileDto>()
                .ForMember(d => d.Id, c => c.MapFrom(d => d.File.Id))
                .ForMember(d => d.Name, c => c.MapFrom(d => d.File.Name))
                .ForMember(d => d.Path, c => c.MapFrom(d => d.File.Path));
            //.ForMember=>d.)
            //.MapExtraProperties();
        }
    }
}
