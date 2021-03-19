using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMVOnline.Guides
{
    public interface IGuideAppService
    {
        Task<GuideDto> GetGuideAsync();
        Task<bool> SetGuideAsync(GuideDto guide);
    }

    public class GuideDto
    {
        public string Content { get; set; }
    }
}
