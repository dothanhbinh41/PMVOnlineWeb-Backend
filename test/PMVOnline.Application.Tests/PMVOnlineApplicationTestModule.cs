using Volo.Abp.Modularity;

namespace PMVOnline
{
    [DependsOn(
        typeof(PMVOnlineApplicationModule),
        typeof(PMVOnlineDomainTestModule)
        )]
    public class PMVOnlineApplicationTestModule : AbpModule
    {

    }
}