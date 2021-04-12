using PMVOnline.Departments;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace PMVOnline.Targets
{
    public interface ITargetAppService
    {
        Task<TargetDto[]> GetAllTargetsAsync();
        Task<TargetDto> CreateTargetsAsync(NameTargetDto request);
        Task<TargetDto> UpdateTargetsAsync(int id, NameTargetDto request);
        Task<bool> DeleteTargetsAsync(int id);


        Task<DepartmentDto[]> GetDepartmentsByTargetAsync(int targetId);
        Task<DepartmentDto[]> AddOrEditDepartmentsToTargetAsync(int targetId, AddOrEditDepartmentsToTargetDto request);
    }

    public class TargetDto : NameTargetDto, IEntityDto<int>
    { 
        public int Id { get; set; }
    }

    public class NameTargetDto
    { 
        public string Name { get; set; }
    }

    public class AddOrEditDepartmentsToTargetDto
    {
        public int[] Departments { set; get; }
    }
}
