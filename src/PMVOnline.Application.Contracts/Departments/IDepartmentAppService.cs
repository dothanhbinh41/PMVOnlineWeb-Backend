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
        Task<DepartmentDto> CreateDepartmentAsync(NameDepartmentDto request);
        Task<DepartmentDto> UpdateDepartmentAsync(int id, NameDepartmentDto request);
        Task<bool> DeleteDepartmentAsync(int id); 
        Task<bool> AddUserToDepartmentAsync(CreateDepartmentUserDto request);
        Task<bool> UpdateUserToDepartmentAsync(UpdateDepartmentUserDto request);
        Task<bool> DeleteUserToDepartmentAsync(DeleteDepartmentUserDto request);
        Task<DepartmentDto[]> GetAllDepartmentsAsync();
        Task<DepartmentUserDto[]> GetDepartmentUsersByIdAsync(int departmentId);
        Task<DepartmentUserDto[]> GetDepartmentUsersByNameAsync(string department);
        Task<DepartmentUserDto[]> GetUserDepartmentsAsync(Guid id);
        Task<DepartmentUserDto[]> GetMyDepartmentsAsync();
    }

    public class DepartmentDto : EntityDto<int>
    {
        public string Name { get; set; }
    }

    public class NameDepartmentDto
    {
        public string Name { get; set; }
    }

    public class DepartmentUserDto : EntityDto<long>
    {
        public int DepartmentId { get; set; }
        public DepartmentDto Department { get; set; }
        public Guid UserId { get; set; }
        public SimpleUserDto User { get; set; }
        public bool IsLeader { get; set; }
    }
    public class DeleteDepartmentUserDto
    {
        public int DepartmentId { get; set; }
        public Guid UserId { get; set; }
    }
    public class CreateDepartmentUserDto
    {
        public int DepartmentId { get; set; }
        public Guid UserId { get; set; }
        public bool IsLeader { get; set; }
    }

    public class UpdateDepartmentUserDto
    {
        public int DepartmentId { get; set; }
        public Guid UserId { get; set; }
        public bool IsLeader { get; set; }
    }

    public class CreateDepartmentNameUserDto
    {
        public string Name { get; set; }
        public bool IsLeader { get; set; }
    }
}
