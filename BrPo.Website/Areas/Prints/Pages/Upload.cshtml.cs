using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BrPo.Website.Areas.Prints.Services;

namespace BrPo.Website.Areas.Prints.Pages
{
    public class UploadModel : PageModel
    {
        private readonly ILogger<UploadModel> _logger;

        public IFormFile UploadedFile { get; set; }

        public UploadModel(
            ILogger<UploadModel> logger)
        {
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into UploadModel");
        }

        public void OnGet()
        {
            _logger.LogInformation("Hello, this is the UploadModel!");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await Task.Delay(10);
                if (UploadedFile == null) return new JsonResult(UploadHelpers.SelectAFile);
            }
            catch (Exception ex)
            {
                _logger.LogError("from UploadModel.OnPostAsync", ex);
                return new JsonResult(UploadHelpers.FileImportError);
            }
            return null;
        }
    }
}
