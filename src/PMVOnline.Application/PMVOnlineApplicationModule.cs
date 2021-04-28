using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PMVOnline.Users;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;

namespace PMVOnline
{
    [DependsOn(
        typeof(PMVOnlineDomainModule),
        typeof(AbpAccountApplicationModule),
        typeof(PMVOnlineApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule)
        )]

    public class PMVOnlineApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<PMVOnlineApplicationModule>();
            });
            //context.Services.AddTransient<IDepartmentIdentityUserAppService, DepartmentIdentityUserAppService>();
            //context.Services.Replace(ServiceDescriptor.Transient<IIdentityUserAppService, UserAppService>()

        }
    }
}
