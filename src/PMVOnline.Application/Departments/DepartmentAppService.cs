using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace PMVOnline.Departments
{
    public class DepartmentAppService : ApplicationService, IDepartmentAppService
    {
        readonly IDepartmentManager departmentManager;

        public DepartmentAppService(IDepartmentManager departmentManager)
        {
            this.departmentManager = departmentManager;
        }

        public Task<bool> AddUserToDeparmentAsync(CreateDeparmentUserDto request)
        {
            return departmentManager.AddUserToDeparmentAsync(ObjectMapper.Map<CreateDeparmentUserDto, DepartmentUser>(request));
        }

        public Task<bool> DeleteUserToDeparmentAsync(DeleteDeparmentUserDto request)
        {
            return departmentManager.DeleteUserToDeparmentAsync(ObjectMapper.Map<DeleteDeparmentUserDto, DepartmentUser>(request));
        }

        public async Task<DepartmentDto[]> GetAllDepartmentAsync()
        {
            return ObjectMapper.Map<Department[], DepartmentDto[]>((await departmentManager.GetAllDepartmentAsync()));
        }

        public async Task<DepartmentUserDto[]> GetAllUserAsync(int departmentId)
        {
            return ObjectMapper.Map<DepartmentUser[], DepartmentUserDto[]>(await departmentManager.GetAllUserAsync(departmentId));
        }

        public async Task<DepartmentUserDto[]> GetAllUserAsync(string department)
        {
            return ObjectMapper.Map<DepartmentUser[], DepartmentUserDto[]>(await departmentManager.GetAllUserAsync(department));
        }

        public Task<bool> UpdateUserToDeparmentAsync(UpdateDeparmentUserDto request)
        {
            return departmentManager.UpdateUserToDeparmentAsync(ObjectMapper.Map<UpdateDeparmentUserDto, DepartmentUser>(request));
        }
    }
}
