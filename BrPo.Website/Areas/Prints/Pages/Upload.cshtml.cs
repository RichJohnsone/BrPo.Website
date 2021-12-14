using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BrPo.Website.Areas.Prints.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using BrPo.Website.Services.Image.Services;

namespace BrPo.Website.Areas.Prints.Pages
{
    public class UploadModel : PageModel
    {
        private readonly ILogger<UploadModel> _logger;
        private readonly IConfiguration _configuration;
        private IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IImageService _imageService;
        public IFormFile UploadFile { get; set; }
        public string AllowedExtensions;
        public string AllowedSize;

        public UploadModel(
            ILogger<UploadModel> logger,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IHttpContextAccessor httpContextAccessor,
            IImageService imageService)
        {
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _imageService = imageService;
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
                    return new BadRequestObjectResult(UploadHelpers.FileExtensionError(_configuration));
                if (!UploadHelpers.CheckSize(UploadFile, _configuration))
                    return new BadRequestObjectResult(UploadHelpers.FileSizeError(_configuration));
                string filePath = UploadHelpers.GetFilePath(UploadFile, _configuration, _environment);
                try
                {
                    var originalFileName = UploadFile.FileName;
                    var userId = Request.Cookies["BrPoSession"].ToString();
                    await UploadHelpers.SaveUploadedFileAsync(UploadFile, filePath);
                    var imageFile = await _imageService.CreateImageRecord(filePath, userId, originalFileName);
                    return new OkObjectResult(imageFile.Id);
                }
                catch (Exception e)
                {
                    _logger.LogError("from UploadModel.OnPostUploadFile", e);
                    return new BadRequestObjectResult(UploadHelpers.FileSaveError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("from UploadModel.OnPostAsync", ex);
                return new BadRequestObjectResult(UploadHelpers.FileUploadError);
            }
        }
    }
}
