using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
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

        public Task<bool> AddUserToDepartmentAsync(CreateDepartmentUserDto request)
        {
            return departmentManager.AddUserToDepartmentAsync(ObjectMapper.Map<CreateDepartmentUserDto, DepartmentUser>(request));
        }

        public async Task<DepartmentDto> CreateDepartmentAsync(NameDepartmentDto request)
        {
            var dep = await departmentManager.CreateDepartmentAsync(new Department { Name = request.Name });
            return ObjectMapper.Map<Department, DepartmentDto>(dep);
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            await departmentManager.DeleteDepartmentAsync(id);
            return true;
        }

        public Task<bool> DeleteUserToDepartmentAsync(DeleteDepartmentUserDto request)
        {
            return departmentManager.DeleteUserToDepartmentAsync(ObjectMapper.Map<DeleteDepartmentUserDto, DepartmentUser>(request));
        }

        public async Task<PagedResultDto<DepartmentDto>> GetAllDepartmentsAsync()
        {
            var deps = await departmentManager.GetAllDepartmentAsync();
            return new PagedResultDto<DepartmentDto>(deps.Length, ObjectMapper.Map<Department[], DepartmentDto[]>(deps));
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

        public async Task<DepartmentDto> UpdateDepartmentAsync(int id, NameDepartmentDto request)
        {
            var dep = await departmentManager.UpdateDepartmentAsync(new Department(id) { Name = request.Name });
            return ObjectMapper.Map<Department, DepartmentDto>(dep);
        }

        public Task<bool> UpdateUserToDepartmentAsync(UpdateDepartmentUserDto request)
        {
            return departmentManager.UpdateUserToDepartmentAsync(ObjectMapper.Map<UpdateDepartmentUserDto, DepartmentUser>(request));
        }
    }
}
