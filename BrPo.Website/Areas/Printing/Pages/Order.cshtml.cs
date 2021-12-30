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
using System.Threading.Tasks;

namespace BrPo.Website.Areas.Printing.Pages
{
    [BindProperties]
    public class OrderModel : PageModel
    {
        private readonly ILogger<UploadModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IImageService _imageService;
        private readonly IPaperService _paperService;
        private readonly UserManager<IdentityUser> _userManager;

        public List<ImageFileModel> Files { get; set; } = new List<ImageFileModel>();
        public ImageFileModel SelectedFile { get; set; }
        public int SelectedPicturePixelHeight { get; set; }
        public int SelectedPicturePixelWidth { get; set; }
        public List<PaperModel> Papers { get; set; } = new List<PaperModel>();
        public int SelectedPaperId { get; set; }
        public List<string> Qualities{ get; set; }
        public string SelectedQuality { get; set; }
        public bool IsDraftPrint { get; set; } = false;
        public int Quantity { get; set; } = 1;

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
                SelectedPicturePixelWidth = Files[0].Width;
                SelectedPicturePixelHeight = Files[0].Height;
            }
            Papers = _paperService.GetPapers();
            SelectedPaperId = Papers[0].Id;
            Qualities = new List<string>() { "Premium", "High" };
            SelectedQuality = "Premium";
        }

        public async Task<IActionResult> OnPostOrderPrints()
        {
            try
            {
                var fileId = Request.Form["SelectedFileId"].ToString().ToInt();
                if (fileId == 0) return new BadRequestObjectResult("Print file not specified");
                var height = Request.Form["height"].ToString().ToInt();
                var width = Request.Form["width"].ToString().ToInt();
                if (height == 0 || width == 0) return new BadRequestObjectResult("Print height or width not specified");
                var paperId = Request.Form["selectedPaperId"].ToString().Split(',')[0].ToInt();
                if (paperId == 0) return new BadRequestObjectResult("Paper not specified");
                var border = Request.Form["border"].ToString().ToInt();
                var quality = Request.Form["selectedQuality"].ToString();
                var draft = Request.Form["isDraftPrint"].ToString();
                var quantity = Request.Form["quantity"].ToString().ToInt();
                if (quantity == 0) return new BadRequestObjectResult("Quantity not specified");
                var paper = await _paperService.GetPaperAsync(paperId);
                var file = await _imageService.GetImageAsync(fileId);
                var orientation = file.Height > file.Width ? "portrait" : "landscape";
                if (!PrintDimenisonsFitOnPaper(height, width, paper, orientation))
                    return new BadRequestObjectResult("Print dimensions too large for paper");
            }
            catch (System.Exception ex)
            {
                return new BadRequestObjectResult("There was an error validating your request information: " + ex.Message);
            }
            try
            {

            }
            catch (System.Exception)
            {

                throw;
            }
            return new OkResult();
        }

        private bool PrintDimenisonsFitOnPaper(int height, int width, PaperModel paper, string orientation)
        {
            var retVal = true;
            var maxRollPaperPrintLength = _configuration["MaxRollPaperPrintLength"].ToString().ToInt();
            var paperHeight = paper.RollPaper ? maxRollPaperPrintLength : paper.CutSheetHeight;
            var paperWidth = paper.RollPaper ? paper.RollWidth : paper.CutSheetWidth;
            if (orientation == "portrait")
            {
                if (height > paperHeight) retVal = false;
                if (width > paperWidth) retVal = false;
            }
            else
            {
                if (width > paperHeight) retVal = false;
                if (height > paperWidth) retVal = false;
            }
            return retVal;
        }

        public PartialViewResult OnGetOrderDisplayPanelPartial(string id)
        {
            var model = _imageService.GetImageAsync(id.ToInt()).Result;
            return Partial("OrderDisplayPanelPartial", model);
        }

        public async Task<IActionResult> OnGetPaper(int id)
        {
            if (id == 0) return new BadRequestObjectResult("You must specify a non-zero id");
            var paper = await _paperService.GetPaperAsync(id);
            if (paper == null) return NotFound();
            return new JsonResult(paper);
        }
    }
}
