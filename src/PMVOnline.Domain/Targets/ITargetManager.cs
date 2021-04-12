using PMVOnline.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace PMVOnline.Targets
{
    public interface ITargetManager
    {
        Task<Department[]> GetDepartmentsByTargetAsync(int targetId);
    }

    public class TargetManager : IDomainService, ITargetManager
    {
        private readonly IRepository<DepartmentTarget, int> departmentTargetsRepostiory;

        public TargetManager(
            IRepository<DepartmentTarget, int> departmentTargetsRepostiory)
        {
            this.departmentTargetsRepostiory = departmentTargetsRepostiory;
        }
        public async Task<Department[]> GetDepartmentsByTargetAsync(int targetId)
        {
            return (await departmentTargetsRepostiory.WithDetailsAsync(d => d.Department)).Where(d => d.TargetId == targetId).ToArray().Select(d => d.Department).ToArray();
        }
    }
}
