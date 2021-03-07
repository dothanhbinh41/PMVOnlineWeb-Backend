using PMVOnline.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace PMVOnline.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(PMVOnlineEntityFrameworkCoreDbMigrationsModule),
        typeof(PMVOnlineApplicationContractsModule)
        )]
    public class PMVOnlineDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
