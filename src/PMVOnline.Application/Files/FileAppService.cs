using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace PMVOnline.Files
{
    public class FileAppService : ApplicationService, IFileAppService
    {
        private readonly IRepository<File, Guid> fileRepostiory;

        public FileAppService(IRepository<PMVOnline.Files.File, Guid> fileRepostiory)
        {
            this.fileRepostiory = fileRepostiory;
        }
        public Task<FileDto> GetFileByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<FileDto> UploadFileAsync(UploadFileDto dto)
        {
            var file = await fileRepostiory.InsertAsync(new File { Name = dto.Name, Size = dto.Content.Length });
            return ObjectMapper.Map<File, FileDto>(file);
        }
    }
}
