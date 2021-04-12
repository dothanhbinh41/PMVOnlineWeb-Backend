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

            await targetRepostiory.InsertManyAsync(new Target[] {
                new Target
                {
                    Name = Buy,
                },
                new Target
                {
                    Name = Pay,
                },
                new Target
                {
                    Name = Check,
                },
                new Target
                {
                    Name = Made,
                },
                new Target
                {
                    Name = Confirm,
                },
                new Target
                {
                    Name = All,
                }
            });

            var buy = await targetRepostiory.FirstOrDefaultAsync(d => d.Name == Buy);
            var depBuy = await departmentRepository.FirstOrDefaultAsync(d => d.Name == DepartmentName.Buy);
            if (buy != null && depBuy != null)
            {
                await departmentTargetRepostiory.InsertAsync(new DepartmentTarget { TargetId = buy.Id, DepartmentId = depBuy.Id });
            }

            var pay = await targetRepostiory.FirstOrDefaultAsync(d => d.Name == Pay);
            var accountant = await departmentRepository.FirstOrDefaultAsync(d => d.Name == DepartmentName.Accountant);
            if (pay != null && accountant != null)
            {
                await departmentTargetRepostiory.InsertAsync(new DepartmentTarget { TargetId = pay.Id, DepartmentId = accountant.Id });
            }

            var check = await targetRepostiory.FirstOrDefaultAsync(d => d.Name == Check);
            var stocker = await departmentRepository.FirstOrDefaultAsync(d => d.Name == DepartmentName.Stocker);
            if (check != null && stocker != null)
            {
                await departmentTargetRepostiory.InsertAsync(new DepartmentTarget { TargetId = check.Id, DepartmentId = stocker.Id });
            }

            var made = await targetRepostiory.FirstOrDefaultAsync(d => d.Name == Made); 
            if (made != null && stocker != null)
            {
                await departmentTargetRepostiory.InsertAsync(new DepartmentTarget { TargetId = made.Id, DepartmentId = stocker.Id });
            }

            var confirm = await targetRepostiory.FirstOrDefaultAsync(d => d.Name == Confirm);
            var director = await departmentRepository.FirstOrDefaultAsync(d => d.Name == DepartmentName.Director);
            if (confirm != null && director != null)
            {
                await departmentTargetRepostiory.InsertAsync(new DepartmentTarget { TargetId = confirm.Id, DepartmentId = director.Id });
            }

            var all = await targetRepostiory.FirstOrDefaultAsync(d => d.Name == All);
            if (all == null)
            {
                return;
            }
            if (buy != null)
            {
                await departmentTargetRepostiory.InsertAsync(new DepartmentTarget { TargetId = all.Id, DepartmentId = depBuy.Id });
            } 
            if (accountant != null)
            {
                await departmentTargetRepostiory.InsertAsync(new DepartmentTarget { TargetId = all.Id, DepartmentId = accountant.Id });
            }
            if (stocker != null)
            {
                await departmentTargetRepostiory.InsertAsync(new DepartmentTarget { TargetId = all.Id, DepartmentId = stocker.Id });
            }
            if (director != null)
            {
                await departmentTargetRepostiory.InsertAsync(new DepartmentTarget { TargetId = all.Id, DepartmentId = director.Id });
            } 
        }
    }
}
