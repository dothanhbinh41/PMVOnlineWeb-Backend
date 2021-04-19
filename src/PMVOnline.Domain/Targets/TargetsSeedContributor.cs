using PMVOnline.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace PMVOnline.Targets
{
    public class TargetsSeedContributor : IDataSeedContributor, ITransientDependency
    {
        readonly IRepository<Department, int> departmentRepository;
        readonly IRepository<Target, int> targetRepostiory;
        readonly IRepository<DepartmentTarget, int> departmentTargetRepostiory;

        public TargetsSeedContributor(
            IRepository<Department, int> departmentRepository,
            IRepository<Target, int> targetRepostiory,
            IRepository<DepartmentTarget, int> departmentTargetRepostiory)
        {
            this.departmentRepository = departmentRepository;
            this.targetRepostiory = targetRepostiory;
            this.departmentTargetRepostiory = departmentTargetRepostiory;
        }


        const string Buy = "Yêu cầu mua hàng";
        const string Pay = "Yêu cầu thanh toán";
        const string Check = "Yêu cầu kiểm tra tồn kho";
        const string Made = "Yêu cầu sản xuất";
        const string Confirm = "Yêu cầu phê duyệt mua sắm";
        const string All = "Yêu cầu khác";
        public async Task SeedAsync(DataSeedContext context)
        {
            if (targetRepostiory.Count() > 0)
            {
                return;
            } 
             
            var depBuy = await departmentRepository.FirstOrDefaultAsync(d => d.Name == DepartmentName.Buy);
            if (depBuy != null)
            {
                await departmentTargetRepostiory.InsertAsync(new DepartmentTarget { Target = new Target
                {
                    Name = Buy,
                }, DepartmentId = depBuy.Id }, true);
            }
             
            var accountant = await departmentRepository.FirstOrDefaultAsync(d => d.Name == DepartmentName.Accountant);
            if ( accountant != null)
            {
                await departmentTargetRepostiory.InsertAsync(new DepartmentTarget { Target = new Target
                {
                    Name = Pay,
                }, DepartmentId = accountant.Id }, true);
            }
             
            var stocker = await departmentRepository.FirstOrDefaultAsync(d => d.Name == DepartmentName.Stocker);
            if (stocker != null)
            {
                await departmentTargetRepostiory.InsertAsync(new DepartmentTarget { Target = new Target
                {
                    Name = Check,
                }, DepartmentId = stocker.Id }, true);
            }
             
            if (stocker != null)
            {
                await departmentTargetRepostiory.InsertAsync(new DepartmentTarget { Target = new Target
                {
                    Name = Made,
                }, DepartmentId = stocker.Id }, true);
            }
             
            var director = await departmentRepository.FirstOrDefaultAsync(d => d.Name == DepartmentName.Director);
            if (director != null)
            {
                await departmentTargetRepostiory.InsertAsync(new DepartmentTarget { Target = new Target
                {
                    Name = Confirm,
                }, DepartmentId = director.Id }, true);
            } 
        }
    }
}
