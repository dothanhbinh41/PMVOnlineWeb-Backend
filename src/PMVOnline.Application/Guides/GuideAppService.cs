using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Settings;
using Volo.Abp.Users;

namespace PMVOnline.Guides
{
    public class GuideAppService : ApplicationService, IGuideAppService
    {
        readonly IRepository<Guide> repository;

        public GuideAppService(IRepository<Guide> repository)
        {
            this.repository = repository;
        }


        [Authorize]
        public async Task<bool> SetGuideAsync(GuideDto guide)
        {
            var uid = CurrentUser.GetId();
            var exist = await repository.GetCountAsync();
            if (exist > 0)
            {
                var g = await repository.FirstOrDefaultAsync();
                g.Content = guide.Content;
                await repository.UpdateAsync(g);
                return true;
            }
            var add = await repository.InsertAsync(new Guide { Content = guide.Content });
            return true;
        }
         
        [AllowAnonymous]
        public async Task<GuideDto> GetGuideAsync()
        {
            var guide = await repository.FirstOrDefaultAsync();
            return ObjectMapper.Map<Guide, GuideDto>(guide);
        }
    }
}
