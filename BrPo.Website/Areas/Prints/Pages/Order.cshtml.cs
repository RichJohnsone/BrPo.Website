using BrPo.Website.Services.Email;
using BrPo.Website.Services.Image.Models;
using BrPo.Website.Services.Image.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrPo.Website.Areas.Prints.Pages
{
    public class OrderModel : PageModel
    {
        private readonly ILogger<UploadModel> _logger;
        private readonly IConfiguration _configuration;
        private IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IImageService _imageService;

        [BindProperty(SupportsGet = true)]
        public List<ImageFileModel> Files { get; set; } = new List<ImageFileModel>();

        public string UploadedFileIds
        {
            get
            {
                object value = _httpContextAccessor.HttpContext.Session.GetString("UploadedFileIds");
                return value == null ? "" : (string)value;
            }
        }

        public OrderModel(
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
        }

        public async Task<IActionResult> OnGetAsync ()
        {
            _httpContextAccessor.HttpContext.Session.SetString("UploadedFileIds", "77");
            if (_httpContextAccessor.HttpContext.Session.GetString("UploadedFileIds") != ""){
                var ids = _httpContextAccessor.HttpContext.Session.GetString("UploadedFileIds").Split(',').ToList().Reverse<string>().ToList();
                foreach (var id in ids)
                {
                    Files.Add(await _imageService.GetImageAsync(id.ToInt()));
                }
            }
            return Page();
        }
    }
}
