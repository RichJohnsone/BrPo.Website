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
using System.Linq;

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

        public string UploadedFileIds
        {
            get
            {
                object value = _httpContextAccessor.HttpContext.Session.GetString("UploadedFileIds");
                return value == null ? "" : (string)value;
            }
            set
            {
                var currentValue = _httpContextAccessor.HttpContext.Session.GetString("UploadedFileIds") ?? string.Empty;
                var newValue = currentValue == string.Empty ? value : currentValue + "," + value;
                _httpContextAccessor.HttpContext.Session.SetString("UploadedFileIds", newValue);
            }
        }

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
            _logger.LogInformation("UploadModel page loaded");
        }

        public async Task<IActionResult> OnPostUploadFile()
        {
            try
            {
                if (UploadFile == null) {
                    return new BadRequestObjectResult(UploadHelpers.SelectAFile);
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
                    UploadedFileIds = imageFile.Id.ToString();
                    return new OkObjectResult(1);
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

        public IActionResult OnGetUploadedImagesPartial()
        {
            var ids = _httpContextAccessor.HttpContext.Session.GetString("UploadedFileIds").Split(',').ToList().Reverse<string>().ToList();
            return Partial("UploadedImagesPartial", ids);
        }
    }
}
