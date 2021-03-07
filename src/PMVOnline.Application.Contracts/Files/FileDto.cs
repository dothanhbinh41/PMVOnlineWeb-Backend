using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace PMVOnline.Files
{
    public class FileDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
