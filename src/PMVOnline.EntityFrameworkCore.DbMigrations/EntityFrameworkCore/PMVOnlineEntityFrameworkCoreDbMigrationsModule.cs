using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace PMVOnline.EntityFrameworkCore
{
    [DependsOn(
        typeof(PMVOnlineEntityFrameworkCoreModule)
        )]
    public class PMVOnlineEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<PMVOnlineMigrationsDbContext>();
        }
    }
}
