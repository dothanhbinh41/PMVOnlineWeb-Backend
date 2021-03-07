using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace PMVOnline
{
    [Dependency(ReplaceServices = true)]
    public class PMVOnlineBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "PMVOnline";
    }
}
