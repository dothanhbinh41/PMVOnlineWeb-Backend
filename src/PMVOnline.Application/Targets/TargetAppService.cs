using Microsoft.AspNetCore.Authorization;
using PMVOnline.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace PMVOnline.Targets
{
    [Authorize]
    public class TargetAppService : ApplicationService, ITargetAppService
    {
        readonly IRepository<Target, int> targetRepostiory;
        readonly IRepository<DepartmentTarget, int> departmentTargetsRepostiory;
        private readonly ITargetManager targetManager;

        public TargetAppService(
            IRepository<Target, int> targetRepostiory,
            IRepository<DepartmentTarget, int> departmentTargetsRepostiory,
            ITargetManager targetManager)
        {
            this.targetRepostiory = targetRepostiory;
            this.departmentTargetsRepostiory = departmentTargetsRepostiory;
            this.targetManager = targetManager;
        }

        public async Task<TargetDto> AddTargetsAsync(AddTargetDto request)
        {
            var result = await departmentTargetsRepostiory.InsertAsync(new DepartmentTarget { Target = new Target { Name = request.Name }, DepartmentId = request.DepartmentId });
            return ObjectMapper.Map<Target, TargetDto>(result.Target);
        }

        public async Task<bool> DeleteTargetsAsync(int id)
        {
            await targetRepostiory.DeleteAsync(id);
            return true;
        }

        public async Task<PagedResultDto<TargetDto>> GetAllTargetsAsync()
        {
            var targets = await targetRepostiory.GetListAsync();
            return new PagedResultDto<TargetDto>(targets.Count, ObjectMapper.Map<List<Target>, TargetDto[]>(targets));
        }

        public async Task<DepartmentDto[]> GetDepartmentsByTargetAsync(int targetId)
        {
            var departments = await targetManager.GetDepartmentsByTargetAsync(targetId);
            return ObjectMapper.Map<Department[], DepartmentDto[]>(departments);
        }

        public async Task<TargetDto> UpdateTargetsAsync(int id, AddTargetDto request)
        {
            var target = await targetRepostiory.GetAsync(id);
            target.Name = request.Name;
            await targetRepostiory.UpdateAsync(target);

            await CurrentUnitOfWork.SaveChangesAsync();
            var department = await departmentTargetsRepostiory.FirstOrDefaultAsync(d => d.TargetId == id && !d.IsDeleted);
            if (department == null)
            {
                await departmentTargetsRepostiory.InsertAsync(new DepartmentTarget { TargetId = id, DepartmentId = request.DepartmentId });
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            if (department.DepartmentId != request.DepartmentId)
            {
                await departmentTargetsRepostiory.DeleteAsync(department);
                await departmentTargetsRepostiory.InsertAsync(new DepartmentTarget { TargetId = id, DepartmentId = request.DepartmentId });
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return ObjectMapper.Map<Target, TargetDto>(target);
        }
    }
}
