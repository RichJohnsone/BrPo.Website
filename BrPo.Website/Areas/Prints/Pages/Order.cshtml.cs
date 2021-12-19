using BrPo.Website.Services.Email;
using BrPo.Website.Services.Image.Models;
using BrPo.Website.Services.Image.Services;
using BrPo.Website.Services.Paper.Models;
using BrPo.Website.Services.Paper.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace BrPo.Website.Areas.Prints.Pages
{
    [BindProperties]
    public class OrderModel : PageModel
    {
        private readonly ILogger<UploadModel> _logger;
        private readonly IConfiguration _configuration;
        private IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IImageService _imageService;
        private readonly IPaperService _paperService;
        private readonly UserManager<IdentityUser> _userManager;

        //[BindProperty(SupportsGet = true)]
        public List<ImageFileModel> Files { get; set; } = new List<ImageFileModel>();

        //[BindProperty(SupportsGet = true)]
        public ImageFileModel SelectedFile { get; set; }

        public List<PaperModel> Papers { get; set; } = new List<PaperModel>();
        public int SelectedPaperId { get; set; }

        public string UploadedFileIds
        {
            get
            {
                object value = _httpContextAccessor.HttpContext.Session.GetString("UploadedFileIds");
                return value == null ? "" : (string)value;
            }
        }
        public string SelectedFileId
        {
            get
            {
                object value = _httpContextAccessor.HttpContext.Session.GetString("SelectedFileId");
                return value == null ? "" : (string)value;
            }
            set
            {
                _httpContextAccessor.HttpContext.Session.SetString("SelectedFileId", value);
            }
        }

        public OrderModel(
            ILogger<UploadModel> logger,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IHttpContextAccessor httpContextAccessor,
            IImageService imageService,
            IPaperService paperService,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _imageService = imageService;
            _paperService = paperService;   
            _userManager = userManager;
        }

        public void OnGet () {
            var principle = this.User;
            if (principle.Identity.IsAuthenticated)
            {
                Files = _imageService.GetImages(_userManager.GetUserId(principle));
            }
            else
            {
                if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("BrPoSession", out var sessionId))
                {
                    Files = _imageService.GetImages(sessionId);
                }
            }
            if (Files.Count > 0) { 
                SelectedFile = Files[0];
                SelectedFileId = Files[0].Id.ToString();
            }

            Papers = _paperService.GetPapers();
        }

        public PartialViewResult OnGetOrderDisplayPanelPartial(string id)
        {
            var model = _imageService.GetImageAsync(id.ToInt()).Result;
            return Partial("OrderDisplayPanelPartial", model);
        }
    }
}
