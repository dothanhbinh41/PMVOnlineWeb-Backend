
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

//namespace PMVOnline.Controllers
//{
   // [Route("api/[controller]/[action]")]
   //// [AbpAuthorize]
   // public class FileController : PMVOnlineController
   // {
   //     private readonly IRepository<PMVOnline.Files.File, Guid> tempRepository;

   //     public FileController(IRepository<PMVOnline.Files.File, Guid> tempRepository)
   //     {
   //         this.tempRepository = tempRepository;
   //     }

   //     [HttpPost]
   //     public async Task<ActionResult<string>> UploadFile()
   //     {
   //         try
   //         {
   //             var profilePictureFile = Request.Form.Files.First();

   //             //Check input
   //             if (profilePictureFile == null)
   //             {
   //                 throw new UserFriendlyException(L("ProfilePicture_Change_Error"));
   //             }

   //             if (profilePictureFile.Length > AppConsts.MaxPictureMegaBytesValue * Math.Pow(2, 20))
   //             {
   //                 throw new UserFriendlyException(L("ProfilePicture_Warn_SizeLimit", AppConsts.MaxPictureMegaBytesValue));
   //             }

   //             byte[] fileBytes;
   //             using (var stream = profilePictureFile.OpenReadStream())
   //             {
   //                 fileBytes = stream.GetAllBytes();
   //                 //using (var resized = ResizeImage(stream, AppConsts.MaxPictureSizeValue))
   //                 //{
   //                 //    fileBytes = resized.GetAllBytes();
   //                 //}
   //             }

   //             //if (!ImageFormatHelper.GetRawImageFormat(fileBytes).IsIn(ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Gif))
   //             //{
   //             //    throw new Exception("Uploaded file is not an accepted image file !");
   //             //}

   //             //Delete old temp profile pictures

   //             var randomName = Guid.NewGuid().ToString();

   //             AppFileHelper.DeleteFilesInFolderIfExists(AppConsts.TempFileFolder, randomName);

   //             //Save new picture
   //             var fileInfo = new FileInfo(profilePictureFile.FileName);
   //             var tempFileName = randomName + fileInfo.Extension;
   //             var tempFilePath = Path.Combine(AppConsts.TempFileFolder, tempFileName);
   //             AppFileHelper.InitFolder(AppConsts.TempFileFolder);
   //             System.IO.File.WriteAllBytes(tempFilePath, fileBytes);
   //             var temp = await tempRepository.InsertAsync(new PMVOnline.Files.File
   //             {
   //                 Path = tempFileName 
   //             });
   //             return new OkObjectResult(temp);
   //         }
   //         catch (UserFriendlyException)
   //         {
   //             return BadRequest();
   //         }
   //     }
   // }
   // public static class AppFileHelper
   // {
   //     public static IEnumerable<string> ReadLines(string path)
   //     {
   //         using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan))
   //         using (var sr = new StreamReader(fs, Encoding.UTF8))
   //         {
   //             string line;
   //             while ((line = sr.ReadLine()) != null)
   //             {
   //                 yield return line;
   //             }
   //         }
   //     }

   //     public static byte[] ReadFile(string path)
   //     {
   //         var filePath = Path.Combine(AppConsts.TempFileFolder, path);
   //         if (!System.IO.File.Exists(filePath))
   //         {
   //             throw new UserFriendlyException("RequestedFileDoesNotExists");
   //         }

   //         var fileBytes = File.ReadAllBytes(filePath);
   //         //File.Delete(filePath);
   //         return fileBytes;
   //     }

   //     public static bool DeleteFile(string path)
   //     {
   //         var filePath = Path.Combine(AppConsts.TempFileFolder, path);
   //         if (!System.IO.File.Exists(filePath))
   //         {
   //             return true;
   //         }
   //         File.Delete(filePath);
   //         return true;
   //     }

   //     public static void DeleteFilesInFolderIfExists(string folderPath, string fileNameWithoutExtension)
   //     {
   //         InitFolder(folderPath);
   //         var directory = new DirectoryInfo(folderPath);
   //         var tempUserProfileImages = directory.GetFiles(fileNameWithoutExtension + ".*", SearchOption.AllDirectories).ToList();
   //         foreach (var tempUserProfileImage in tempUserProfileImages)
   //         {
   //             FileHelper.DeleteIfExists(tempUserProfileImage.FullName);
   //         }
   //     }

   //     public static void InitFolder(string folderPath)
   //     {
   //         try
   //         {
   //             if (!Directory.Exists(folderPath))
   //             {
   //                 Directory.CreateDirectory(folderPath);
   //             }
   //         }
   //         catch (Exception)
   //         { }

   //         return;
   //     }
   // }
