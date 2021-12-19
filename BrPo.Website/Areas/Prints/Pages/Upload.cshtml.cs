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
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BrPo.Website.Areas.Prints.Pages
{
    public class UploadModel : PageModel
    {
        private readonly ILogger<UploadModel> _logger;
        private readonly IConfiguration _configuration;
        private IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IImageService _imageService;
        private readonly UserManager<IdentityUser> _userManager;

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
            IImageService imageService, 
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _imageService = imageService;
            _userManager = userManager;
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
                    string userId = GetUserOrSessionId();
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

        private string GetUserOrSessionId()
        {
            var principle = this.User;
            if (principle.Identity.IsAuthenticated)
            {
                return _userManager.GetUserId(principle);
            }
            else
            {
                if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("BrPoSession", out var sessionId))
                    return sessionId.ToString();
                else
                {
                    throw new ApplicationException("Your session has expired");
                }
            }
        }

        public IActionResult OnGetUploadedImagesPartial()
        {
            var ids = new List<string>();
            var principle = this.User;
            if (principle.Identity.IsAuthenticated)
            {
                ids = _imageService.GetIds(_userManager.GetUserId(principle));
            }
            else
            {
                if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("BrPoSession", out var sessionId))
                {
                    ids = _imageService.GetIds(sessionId);
                }
            }
            return Partial("UploadedImagesPartial", ids);
        }
    }
}
