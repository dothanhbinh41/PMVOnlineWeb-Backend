using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PMVOnline.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace PMVOnline.Users
{
    public interface IDepartmentIdentityUserAppService
    {
        Task<IdentityUserDto> CreateNewAsync(UserDepartmentCreateDto input);
    }

    public class DepartmentIdentityUserAppService : ApplicationService, IDepartmentIdentityUserAppService
    {
        readonly IDepartmentManager departmentManager;
        private readonly IIdentityUserAppService identityUserApp;

        public DepartmentIdentityUserAppService(IDepartmentManager departmentManager, IIdentityUserAppService identityUserApp)
        {
            this.departmentManager = departmentManager;
            this.identityUserApp = identityUserApp;
        }
        public async Task<IdentityUserDto> CreateNewAsync(UserDepartmentCreateDto input)
        {
            var result = await identityUserApp.CreateAsync(input); 
            await CreateDepartments(input.Departments, result.Id);
            return result;
        }

        async Task CreateDepartments(CreateDepartmentNameUserDto[] departments, Guid uid)
        {
            if (departments == null || departments.Length == 0)
            {
                return;
            }

            var dep = departments.Select(d => new DepartmentUser { Department = departmentManager.GetDepartmentByName(d.Name), IsLeader = d.IsLeader, UserId = uid });
            await departmentManager.AddUserToDeparmentAsync(dep.ToArray());
        }

    }

    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IIdentityUserAppService), typeof(IdentityUserAppService), typeof(UserAppService))]
    public class UserAppService : IdentityUserAppService, ITransientDependency
    {
        readonly IRepository<AppUser, Guid> appUsersRepository;
        readonly IDepartmentManager departmentManager;

        public UserAppService(
            IRepository<AppUser, Guid> appUsersRepository,
            IdentityUserManager userManager,
            IIdentityUserRepository userRepository,
            IIdentityRoleRepository roleRepository,
            IOptions<IdentityOptions> identityOptions,
            IDepartmentManager departmentManager) : base(userManager, userRepository, roleRepository, identityOptions)
        {
            this.appUsersRepository = appUsersRepository;
            this.departmentManager = departmentManager;
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
