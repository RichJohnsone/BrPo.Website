using System.Collections.Generic;
using BrPo.Website.Areas.Sell.Models;
using BrPo.Website.Services.ApplicationUser.Models;
using BrPo.Website.Services.ApplicationUser.Services;
using BrPo.Website.Services.Image.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BrPo.Website.Areas.Sell.Pages.Public;

public class GalleryViewModel : PageModel
{
    private readonly ILogger<GalleryViewModel> _logger;
    private readonly IImageService _imageService;
    private readonly IApplicationUserService _applicationUserService;

    public ApplicationUser ApplicationUser { get; set; }

    [BindProperty(SupportsGet = true)]
    public string GalleryRootName { get; set; }

    [BindProperty(SupportsGet = true)]
    public string GalleryName { get; set; }

    public GalleryViewModel(
        ILogger<GalleryViewModel> logger,
        IImageService imageService,
        IApplicationUserService applicationUserService)
    {
        _logger = logger;
        _imageService = imageService;
        _applicationUserService = applicationUserService;
    }

    public void OnGet()
    {
    }

    public PartialViewResult OnGetGalleryPartial(string galleryRootName, string gallName, int viewHeight, int viewWidth)
    {
        var gallery = _imageService.GetGalleryAsync(galleryRootName, gallName).Result;
        if (gallery == null) return Partial("NotFoundPartial", Request.Path);
        var content = new List<CarouselContentModel>();
        if (gallery.ContentCount > 0)
        {
            foreach (var item in gallery.Content)
            {
                var contentModel = new CarouselContentModel()
                {
                    Name = item.ImageGalleryItem.Name,
                    Description = item.ImageGalleryItem.Description,
                    ImageFileId = item.ImageGalleryItem.ImageFileId,
                    ImageGalleryItemId = item.ImageGalleryItemId,
                    Link = $"/Print/Order?handler=BuyPrint&galleryItemId={item.ImageGalleryItemId}",
                    LinkClass = "",
                    LinkTitle = "Buy a print",
                    ViewHeight = viewHeight,
                    ViewWidth = viewWidth,
                    IsPortrait = item.ImageGalleryItem.ImageFile.Height > item.ImageGalleryItem.ImageFile.Width
                };
                content.Add(contentModel);
            }
        }
        var carousel = new CarouselModel();
        carousel.Content = content;
        carousel.CarouselId = "gallery-carousel";
        carousel.ContentHeight = 500;
        carousel.IndicatorHeight = 80;
        carousel.ShowContent = true;
        carousel.ShowIndicators = true;
        return Partial("CarouselPartial", carousel);
    }

    public IActionResult OnGetImageAsync(int imageId, int height, int width)
    {
        if (imageId == 0) return null;
        return File(_imageService.GetImageAsync(imageId, height, width).Result, "image/jpeg");
    }
}