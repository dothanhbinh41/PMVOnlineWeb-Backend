
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMVOnline.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.IO;

namespace PMVOnline.Controllers
{
    [Route("api/[controller]/[action]")]
    // 
    public class FileController : PMVOnlineController
    {
        public const int MaxPictureMegaBytesValue = 10;
        public const int MaxPictureSizeValue = 480;

        private readonly IRepository<PMVOnline.Files.File, Guid> tempRepository;
        private readonly IGuidGenerator guidGenerator;

        public FileController(
            IRepository<Files.File, Guid> tempRepository,
            IGuidGenerator guidGenerator
            )
        {
            this.tempRepository = tempRepository;
            this.guidGenerator = guidGenerator;
        }

        // [Authorize]
        [HttpPost]
        public async Task<ActionResult<string>> UploadFile()
        {
            try
            {
                var profilePictureFile = Request.Form.Files.FirstOrDefault();

                //Check input
                if (profilePictureFile == null)
                {
                    throw new UserFriendlyException("ProfilePicture_Change_Error");
                }

                if (profilePictureFile.Length > MaxPictureMegaBytesValue * Math.Pow(2, 20))
                {
                    throw new UserFriendlyException("400");
                }

                byte[] fileBytes;
                using (var stream = profilePictureFile.OpenReadStream())
                {
                    fileBytes = stream.GetAllBytes();
                    //using (var resized = ResizeImage(stream, AppConsts.MaxPictureSizeValue))
                    //{
                    //    fileBytes = resized.GetAllBytes();
                    //}
                }

                //if (!ImageFormatHelper.GetRawImageFormat(fileBytes).IsIn(ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Gif))
                //{
                //    throw new Exception("Uploaded file is not an accepted image file !");
                //}

                //Delete old temp profile pictures

                var randomName = Guid.NewGuid().ToString();

                AppFileHelper.DeleteFilesInFolderIfExists(PMVOnlineConst.FileFolder, randomName);

                //Save new picture
                var fileInfo = new FileInfo(profilePictureFile.FileName);
                var tempFileName = randomName + fileInfo.Extension;
                var tempFilePath = Path.Combine(PMVOnlineConst.FileFolder, tempFileName);
                AppFileHelper.InitFolder(PMVOnlineConst.FileFolder);
                System.IO.File.WriteAllBytes(tempFilePath, fileBytes);
                var temp = await tempRepository.InsertAsync(new PMVOnline.Files.File(guidGenerator.Create())
                {
                    Name = profilePictureFile.FileName,
                    Size = fileBytes.Length,
                    Path = tempFileName
                });
                return new OkObjectResult(ObjectMapper.Map<Files.File, FileDto>(temp));
            }
            catch (UserFriendlyException)
            {
                return BadRequest();
            }
        }

        // [Authorize]
        [HttpGet]
        public async Task<ActionResult<string>> DownloadFile(Guid id)
        {
            try
            {
                var file = await tempRepository.GetAsync(id); 
                var fileInfo = new FileInfo(file.Name); 
                var tempFilePath = Path.Combine(PMVOnlineConst.FileFolder, file.Path);
                AppFileHelper.InitFolder(PMVOnlineConst.FileFolder);
                var ff = new FileStream(tempFilePath, FileMode.Open);
                return new FileStreamResult(ff, "multipart/form-data") { FileDownloadName = file.Name };
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
    }
    public static class AppFileHelper
    {
        public static IEnumerable<string> ReadLines(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public static byte[] ReadFile(string path)
        {
            var filePath = Path.Combine(PMVOnlineConst.FileFolder, path);
            if (!System.IO.File.Exists(filePath))
            {
                throw new UserFriendlyException("RequestedFileDoesNotExists");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            //File.Delete(filePath);
            return fileBytes;
        }

        public static bool DeleteFile(string path)
        {
            var filePath = Path.Combine(PMVOnlineConst.FileFolder, path);
            if (!System.IO.File.Exists(filePath))
            {
                return true;
            }
            System.IO.File.Delete(filePath);
            return true;
        }

        public static void DeleteFilesInFolderIfExists(string folderPath, string fileNameWithoutExtension)
        {
            InitFolder(folderPath);
            var directory = new DirectoryInfo(folderPath);
            var tempUserProfileImages = directory.GetFiles(fileNameWithoutExtension + ".*", SearchOption.AllDirectories).ToList();
            foreach (var tempUserProfileImage in tempUserProfileImages)
            {
                FileHelper.DeleteIfExists(tempUserProfileImage.FullName);
            }
        }

        public static void InitFolder(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
            }
            catch (Exception)
            { }

            return;
        }
    }
}
