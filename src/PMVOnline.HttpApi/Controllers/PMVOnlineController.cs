using PMVOnline.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace PMVOnline.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class PMVOnlineController : AbpController
    {
        protected PMVOnlineController()
        {
            LocalizationResource = typeof(PMVOnlineResource);
        }
    }
}