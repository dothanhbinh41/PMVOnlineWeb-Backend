using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PMVOnline.Data;
using Volo.Abp.DependencyInjection;

namespace PMVOnline.EntityFrameworkCore
{
    public class EntityFrameworkCorePMVOnlineDbSchemaMigrator
        : IPMVOnlineDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCorePMVOnlineDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the PMVOnlineMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<PMVOnlineMigrationsDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}