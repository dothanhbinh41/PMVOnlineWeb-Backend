using System;
using System.Collections.Generic;
using System.Text;
using PMVOnline.Localization;
using Volo.Abp.Application.Services;

namespace PMVOnline
{
    /* Inherit your application services from this class.
     */
    public abstract class PMVOnlineAppService : ApplicationService
    {
        protected PMVOnlineAppService()
        {
            LocalizationResource = typeof(PMVOnlineResource);
        }
    }
}
