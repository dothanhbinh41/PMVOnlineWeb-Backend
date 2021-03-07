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
    public class RolesSeedContributor : IDataSeedContributor, ITransientDependency
    {
        readonly IRepository<IdentityRole, Guid> roleRepository;
        readonly IGuidGenerator guidGenerator;

        public RolesSeedContributor(
            IRepository<IdentityRole, Guid> roleRepository,
            IGuidGenerator guidGenerator
            )
        {
            this.roleRepository = roleRepository;
            this.guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {

            if (roleRepository.Count() > 1)
            {
                return;
            }

            await roleRepository.InsertManyAsync(
                new IdentityRole[] {
                    new IdentityRole(guidGenerator.Create(), RoleName.Account) ,
                    new IdentityRole(guidGenerator.Create(), RoleName.Buy) ,
                    new IdentityRole(guidGenerator.Create(), RoleName.Storage)
                });
        }
    }
}
