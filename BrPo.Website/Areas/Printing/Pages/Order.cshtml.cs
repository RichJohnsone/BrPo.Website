using BrPo.Website.Services.ApplicationUser.Models;
using BrPo.Website.Services.ApplicationUser.Services;
using BrPo.Website.Services.Email;
using BrPo.Website.Services.Image.Models;
using BrPo.Website.Services.Image.Services;
using BrPo.Website.Services.Paper.Models;
using BrPo.Website.Services.Paper.Services;
using BrPo.Website.Services.ShoppingBasket.Models;
using BrPo.Website.Services.ShoppingBasket.Services;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IImageService _imageService;
        private readonly IPaperService _paperService;
        private readonly IShoppingBasketService _shoppingBasketService;
        private readonly IApplicationUserService _applicationUserService;

        public ApplicationUser ApplicationUser { get; set; }
        public List<ImageFileModel> Files { get; set; } = new List<ImageFileModel>();
        public ImageFileModel SelectedFile { get; set; }
        public int SelectedPicturePixelHeight { get; set; }
        public int SelectedPicturePixelWidth { get; set; }
        public List<PaperModel> Papers { get; set; } = new List<PaperModel>();
        public int SelectedPaperId { get; set; }
        public List<string> Qualities { get; set; }
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
            IHttpContextAccessor httpContextAccessor,
            IImageService imageService,
            IPaperService paperService,
            IShoppingBasketService shoppingBasketService,
            IApplicationUserService applicationUserService)
        {
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _imageService = imageService;
            _paperService = paperService;
            _shoppingBasketService = shoppingBasketService;
            _applicationUserService = applicationUserService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser = await _applicationUserService.GetCurrentUserAsync(this.User);
            Files = _imageService.GetImages(ApplicationUser.Id.ToString());
            InitialiseControls();
            return Page();
        }

        public async Task<IActionResult> OnGetBuyPrintAsync(int galleryItemId)
        {
            ApplicationUser = await _applicationUserService.GetCurrentUserAsync(this.User);
            Files = await _imageService.GetImage(galleryItemId);
            InitialiseControls();
            return Page();
        }

        private void InitialiseControls()
        {
            if (Files.Count > 0)
            {
                SelectedFile = Files[0];
                SelectedFileId = Files[0].Id.ToString();
                SelectedPicturePixelWidth = Files[0].Width;
                SelectedPicturePixelHeight = Files[0].Height;
            }
            Papers = _paperService.GetPapers();
            if (Papers.Count > 0)
                SelectedPaperId = Papers[0].Id;
            Qualities = new List<string>() { "Premium", "High" };
            SelectedQuality = "Premium";
        }

        public async Task<IActionResult> OnPostOrderPrints()
        {
            ApplicationUser = await _applicationUserService.GetCurrentUserAsync(this.User);
            var printOrder = new PrintOrderItem();
            try
            {
                printOrder.FileId = Request.Form["SelectedFileId"].ToString().ToInt();
                if (printOrder.FileId == 0) return new BadRequestObjectResult("Print file not specified");
                printOrder.Height = Request.Form["height"].ToString().ToInt();
                printOrder.Width = Request.Form["width"].ToString().ToInt();
                if (printOrder.Height == 0 || printOrder.Width == 0) return new BadRequestObjectResult("Print height or width not specified");
                printOrder.PaperId = Request.Form["selectedPaperId"].ToString().Split(',')[0].ToInt();
                if (printOrder.PaperId == 0) return new BadRequestObjectResult("Paper not specified");
                printOrder.Border = Request.Form["border"].ToString().ToInt();
                printOrder.Quality = Request.Form["selectedQuality"].ToString();
                printOrder.IsDraft = Request.Form["isDraftPrint"].ToString().Contains("true");
                printOrder.Quantity = Request.Form["quantity"].ToString().ToInt();
                if (printOrder.Quantity == 0) return new BadRequestObjectResult("Quantity not specified");
                printOrder.Value = Request.Form["orderValue"].ToString().ToCurrency();
                printOrder.UserId = ApplicationUser.Id;
                var paper = await _paperService.GetPaperAsync(printOrder.PaperId);
                if (!_shoppingBasketService.PriceIsCorrect(printOrder, paper))
                    return new BadRequestObjectResult("Price discrepancy");
                var file = await _imageService.GetImageFileModelAsync(printOrder.FileId);
                var orientation = file.Height > file.Width ? "portrait" : "landscape";
                if (!PrintDimenisonsFitOnPaper(printOrder.Height, printOrder.Width, paper, orientation))
                    return new BadRequestObjectResult("Print dimensions too large for paper");
            }
            catch (System.Exception ex)
            {
                _logger.LogError("from Order.OnPostOrderPrints", ex);
                return new BadRequestObjectResult("There was an error validating your order information: " + ex.Message);
            }
            try
            {
                await _shoppingBasketService.CreatePrintOrderItemAsync(printOrder);
                await _shoppingBasketService.AddPrintOrderItemToBasketAsync(printOrder);
            }
            catch (System.Exception ex)
            {
                return new BadRequestObjectResult("There was an error saving your order information: " + ex.Message);
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
            var model = _imageService.GetImageFileModelAsync(id.ToInt()).Result;
            return Partial("OrderDisplayPanelPartial", model);
        }

        public async Task<IActionResult> OnGetPaper(int id)
        {
            if (id == 0) return new BadRequestObjectResult("You must specify a non-zero id");
            var paper = await _paperService.GetPaperAsync(id);
            if (paper == null) return NotFound();
            return new JsonResult(paper);
        }

        public async Task<IActionResult> OnGetBasketCountAsync()
        {
            ApplicationUser = await _applicationUserService.GetCurrentUserAsync(this.User);
            var count = 0;
            if (ApplicationUser != null)
                count = _shoppingBasketService.GetBasketCount(ApplicationUser.Id.ToString());
            return new JsonResult(count);
        }
    }
}