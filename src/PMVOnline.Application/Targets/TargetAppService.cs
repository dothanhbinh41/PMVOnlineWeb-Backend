using Microsoft.AspNetCore.Authorization;
using PMVOnline.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<DepartmentDto[]> AddOrEditDepartmentsToTargetAsync(int targetId, AddOrEditDepartmentsToTargetDto request)
        {
            var departments = departmentTargetsRepostiory.Where(d => d.TargetId == targetId).ToArray() ?? new DepartmentTarget[0];
            var removed = departments.Where(d => !request.Departments.Any(c => c == d.DepartmentId)).ToArray() ?? new DepartmentTarget[0];
            var added = request.Departments.Where(d => !departments.Any(c => c.DepartmentId == d)).ToArray()??new int[0];
            if (removed.Length > 0)
            {
                await departmentTargetsRepostiory.DeleteManyAsync(removed); 
            }
            if (added.Length > 0)
            { 
                await departmentTargetsRepostiory.InsertManyAsync(added.Select(d => new DepartmentTarget { TargetId = targetId, DepartmentId = d }));
            }

            var deps = (await departmentTargetsRepostiory.WithDetailsAsync(d => d.Department)).Where(d => d.TargetId == targetId).ToArray().Select(d=>d.Department).ToArray();

            return ObjectMapper.Map<Department[], DepartmentDto[]>(deps);
        }

        public async Task<TargetDto> CreateTargetsAsync(NameTargetDto request)
        {
            var target = await targetRepostiory.InsertAsync(new Target { Name = request.Name });
            return ObjectMapper.Map<Target, TargetDto>(target);
        }

        public async Task<bool> DeleteTargetsAsync(int id)
        {
            await targetRepostiory.DeleteAsync(id);
            return true;
        }

        public async Task<TargetDto[]> GetAllTargetsAsync()
        {
            var targets = await targetRepostiory.GetListAsync();
            return ObjectMapper.Map<List<Target>, TargetDto[]>(targets);
        }

        public async Task<DepartmentDto[]> GetDepartmentsByTargetAsync(int targetId)
        {
            var departments = await targetManager.GetDepartmentsByTargetAsync(targetId);
            return ObjectMapper.Map<Department[], DepartmentDto[]>(departments);
        }

        public async Task<TargetDto> UpdateTargetsAsync(int id, NameTargetDto request)
        {
            var target = await targetRepostiory.GetAsync(id);
            target.Name = request.Name;
            await targetRepostiory.UpdateAsync(target);
            return ObjectMapper.Map<Target, TargetDto>(target);
        }
    }
}
