using PMVOnline.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;

namespace PMVOnline.Departments
{
    public class DepartmentsSeedContributor : IDataSeedContributor, ITransientDependency
    {
        readonly IRepository<Department, int> departmentRepository;
        readonly IRepository<DepartmentUser, long> departmentUserRepository;
        readonly IRepository<IdentityUser, Guid> appUserRepository;

        public DepartmentsSeedContributor(
            IRepository<Department, int> departmentRepository,
            IRepository<IdentityUser, Guid> appUserRepository,
            IRepository<DepartmentUser, long> departmentUserRepository
            )
        {
            this.departmentRepository = departmentRepository;
            this.appUserRepository = appUserRepository;
            this.departmentUserRepository = departmentUserRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {

            if (departmentRepository.Count() > 1)
            {
                return;
            }
            var admin = await appUserRepository.FirstOrDefaultAsync();
            await departmentUserRepository.InsertAsync(new DepartmentUser { UserId = admin.Id, Department = new Department { Name = DepartmentName.Director } });
            await departmentRepository.InsertManyAsync(
                new Department[] {
                    new Department{ Name= DepartmentName.Accountant } ,
                    new Department{ Name= DepartmentName.Buy } ,
                    new Department{ Name= DepartmentName.Stocker }
                }); 
        }
    }
}
