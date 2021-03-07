using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMVOnline.Files
{
    public interface IFileAppService
    {
        Task<FileDto> GetFileByIdAsync(Guid id);
        Task<FileDto> UploadFileAsync(UploadFileDto dto);
    }
}
