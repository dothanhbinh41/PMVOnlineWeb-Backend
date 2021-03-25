using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace PMVOnline.Departments
{
    [Authorize]
    public class DepartmentAppService : ApplicationService, IDepartmentAppService
    {
        readonly IDepartmentManager departmentManager;

        public DepartmentAppService(IDepartmentManager departmentManager)
        {
            this.departmentManager = departmentManager;
        }

        public Task<bool> AddUserToDeparmentAsync(CreateDepartmentUserDto request)
        {
            return departmentManager.AddUserToDeparmentAsync(ObjectMapper.Map<CreateDepartmentUserDto, DepartmentUser>(request));
        }

        public Task<bool> DeleteUserToDeparmentAsync(DeleteDepartmentUserDto request)
        {
            return departmentManager.DeleteUserToDeparmentAsync(ObjectMapper.Map<DeleteDepartmentUserDto, DepartmentUser>(request));
        }

        public async Task<DepartmentDto[]> GetAllDepartmentsAsync()
        {
            return ObjectMapper.Map<Department[], DepartmentDto[]>((await departmentManager.GetAllDepartmentAsync()));
        }

        public async Task<DepartmentUserDto[]> GetDepartmentUsersByIdAsync(int departmentId)
        {
            return ObjectMapper.Map<DepartmentUser[], DepartmentUserDto[]>(await departmentManager.GetAllUserAsync(departmentId));
        }

        public async Task<DepartmentUserDto[]> GetDepartmentUsersByNameAsync(string department)
        {
            return ObjectMapper.Map<DepartmentUser[], DepartmentUserDto[]>(await departmentManager.GetAllUserAsync(department));
        }

        public Task<DepartmentUserDto[]> GetMyDepartmentsAsync()
        {
            return GetUserDepartmentsAsync(CurrentUser.GetId());
        }

        public async Task<DepartmentUserDto[]> GetUserDepartmentsAsync(Guid id)
        {
            return ObjectMapper.Map<DepartmentUser[], DepartmentUserDto[]>(await departmentManager.GetUserDepartmentsAsync(id));
        }

        public Task<bool> UpdateUserToDeparmentAsync(UpdateDepartmentUserDto request)
        {
            return departmentManager.UpdateUserToDeparmentAsync(ObjectMapper.Map<UpdateDepartmentUserDto, DepartmentUser>(request));
        }
    }
}
