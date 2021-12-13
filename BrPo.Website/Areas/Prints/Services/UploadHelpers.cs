using BrPo.Website.Services.Email;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BrPo.Website.Areas.Prints.Services
{
    public class UploadHelpers
    {
        public static string Success => "Your file has bee uploaded.";
        public static string SelectAFile => "You must select a file to be imported.";
        public static string FileSaveError => "File could not be saved. Please try again.";
        public static string FileUploadError => "File could not be uploaded. Please try again.";

        public static string FileExtensionError(IConfiguration configuration)
        {
            return $"Incorect file type. Allowed types are: {GetAllowedExtensions(configuration)}";
        }

        public static string FileSizeError(IConfiguration configuration)
        {
            return $"File is too large. maximum allowed size is: {GetMaxAllowedSize(configuration)}";
        }

        public static string GetMaxAllowedSize(IConfiguration configuration)
        {
            return configuration["FileUpload:maxAllowedSizeinMB"].ToString();
        }

        public static string GetAllowedExtensions(IConfiguration configuration)
        {
            return configuration["FileUpload:AllowedFileExtensions"].ToString();
        }

        public static bool CheckExtension(IFormFile file, IConfiguration configuration)
        {
            var fileName = file.FileName;
            var fileExtension = Path.GetExtension(fileName);
            return GetAllowedExtensions(configuration).Contains(fileExtension);
        }

        public static bool CheckSize(IFormFile file, IConfiguration configuration)
        {
            var fileSize = file.Length;
            var maxAllowedSizeinMB = GetMaxAllowedSize(configuration).ToInt();
            return (fileSize / 1024) < maxAllowedSizeinMB;
        }

        public static string GetFilePath(IFormFile file, IConfiguration configuration, IWebHostEnvironment environment)
        {
            var uploadDirectory = configuration["FileUpload:FolderPath"];
            var webRootPath = environment.EnvironmentName == "Development" ? environment.ContentRootPath + "\\bin\\Debug\\net5.0" : environment.WebRootPath; 
            if (!Directory.Exists(Path.Combine(webRootPath, uploadDirectory))) 
                Directory.CreateDirectory(Path.Combine(webRootPath, uploadDirectory));
            var timeStamp = DateTime.UtcNow.ToString("yyMMddHHmmss");
            var fileName = "Image" + "_" + timeStamp + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(webRootPath, uploadDirectory, fileName);
            return fullPath;
        }

        public static async Task SaveUploadedFileAsync(IFormFile file, string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }
    }
}
