using PMVOnline.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace PMVOnline.Departments
{
    public interface IDepartmentAppService
    {
        Task<bool> AddUserToDeparmentAsync(CreateDeparmentUserDto request);
        Task<bool> UpdateUserToDeparmentAsync(UpdateDeparmentUserDto request);
        Task<bool> DeleteUserToDeparmentAsync(DeleteDeparmentUserDto request);
        Task<DepartmentDto[]> GetAllDepartmentsAsync();
        Task<DepartmentUserDto[]> GetDepartmentUsersAsync(int departmentId);
        Task<DepartmentUserDto[]> GetDepartmentUsersAsync(string department);
        Task<DepartmentUserDto[]> GetUserDepartmentsAsync();
    }

    public class DepartmentDto : EntityDto<int>
    {
        public string Name { get; set; }
    }


    public class DepartmentUserDto : EntityDto<long>
    {
        public int DeparmentId { get; set; }
        public DepartmentDto Deparment { get; set; }
        public Guid UserId { get; set; }
        public SimpleUserDto User { get; set; }
        public bool IsLeader { get; set; }
    }
    public class DeleteDeparmentUserDto
    {
        public int DeparmentId { get; set; }
        public Guid UserId { get; set; }
    }
    public class CreateDeparmentUserDto
    {
        public int DeparmentId { get; set; }
        public Guid UserId { get; set; }
        public bool IsLeader { get; set; }
    }

    public class UpdateDeparmentUserDto
    {
        public int DeparmentId { get; set; }
        public Guid UserId { get; set; }
        public bool IsLeader { get; set; }
    }
}
