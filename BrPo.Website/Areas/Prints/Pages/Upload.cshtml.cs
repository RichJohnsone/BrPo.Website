using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BrPo.Website.Areas.Prints.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace BrPo.Website.Areas.Prints.Pages
{
    public class UploadModel : PageModel
    {
        private readonly ILogger<UploadModel> _logger;
        private readonly IConfiguration _configuration;
        private IWebHostEnvironment _environment;

        public IFormFile UploadFile { get; set; }
        public string AllowedExtensions;
        public string AllowedSize;

        public UploadModel(
            ILogger<UploadModel> logger,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
            AllowedExtensions = UploadHelpers.GetAllowedExtensions(_configuration);
            AllowedSize = UploadHelpers.GetMaxAllowedSize(_configuration);
        }

        public void OnGet()
        {
            _logger.LogInformation("Hello, this is the UploadModel!");
        }

        public async Task<IActionResult> OnPostUploadFile()
        {
            try
            {
                if (UploadFile == null) {
                    return new BadRequestObjectResult(new JsonResult(UploadHelpers.SelectAFile));
                }
                _logger.LogInformation($"FileInfo: name: {UploadFile.FileName} - Size: {UploadFile.Length}");
                if (!UploadHelpers.CheckExtension(UploadFile, _configuration))
                    return new BadRequestObjectResult(new JsonResult(UploadHelpers.FileExtensionError(_configuration)));
                if (UploadHelpers.CheckSize(UploadFile, _configuration))
                    return new BadRequestObjectResult(new JsonResult(UploadHelpers.FileSizeError(_configuration)));
                string filePath = UploadHelpers.GetFilePath(UploadFile, _configuration, _environment);
                try
                {
                    await UploadHelpers.SaveUploadedFileAsync(UploadFile, filePath);

                    return new OkObjectResult(filePath);
                }
                catch (Exception e)
                {
                    _logger.LogError("from UploadModel.OnPostUploadFile", e);
                    return new BadRequestObjectResult(new JsonResult(UploadHelpers.FileSaveError));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("from UploadModel.OnPostAsync", ex);
                return new BadRequestObjectResult(new JsonResult(UploadHelpers.FileUploadError));
            }
        }
    }
}
