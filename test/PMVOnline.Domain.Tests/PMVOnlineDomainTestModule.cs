using PMVOnline.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace PMVOnline
{
    [DependsOn(
        typeof(PMVOnlineEntityFrameworkCoreTestModule)
        )]
    public class PMVOnlineDomainTestModule : AbpModule
    {

    }
}