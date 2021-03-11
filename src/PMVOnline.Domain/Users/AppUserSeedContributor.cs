using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;

namespace PMVOnline.Users
{
    public class AppUserSeedContributor : IDataSeedContributor, ITransientDependency
    {
        readonly IRepository<IdentityUser, Guid> userRepository;
        readonly IRepository<AppUser, Guid> appUserRepository;
        readonly IObjectMapper<PMVOnlineDomainModule> objectMapper;

        public AppUserSeedContributor(
            IRepository<IdentityUser, Guid> userRepository,
            IRepository<AppUser, Guid> appUserRepository,
            IObjectMapper<PMVOnlineDomainModule> objectMapper
            )
        {
            this.userRepository = userRepository;
            this.appUserRepository = appUserRepository;
            this.objectMapper = objectMapper;
        }

        public async Task SeedAsync(DataSeedContext context)
        {

            if (appUserRepository.Count() == userRepository.Count())
            {
                return;
            } 
            var appUsers = (await appUserRepository.GetListAsync()).Select(d=>d.Id).ToArray(); 
            await appUserRepository.InsertManyAsync(objectMapper.Map<IdentityUser[], AppUser[]>(userRepository.Where(d => !appUsers.Contains(d.Id)).ToArray()));
        }
    }
}
