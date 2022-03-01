using System.Collections.Generic;
using System.Threading.Tasks;
using BrPo.Website.Areas.Sell.Models;
using BrPo.Website.Areas.Sell.Pages;
using BrPo.Website.Services.ApplicationUser.Services;
using BrPo.Website.Services.Image.Models;
using BrPo.Website.Services.Image.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BrPo.Website.Areas.Buy.Pages;

[BindProperties]
public class GalleriesModel : PageModel
{
    private readonly ILogger<MyImagesModel> _logger;
    private readonly IImageService _imageService;
    private readonly IApplicationUserService _applicationUserService;

    private List<ImageGallery> Galleries { get; set; }
    public CarouselModel CarouselModel { get; set; } = new CarouselModel();

    public GalleriesModel(
        ILogger<MyImagesModel> logger,
        IImageService imageService,
        IApplicationUserService applicationUserService)
    {
        _logger = logger;
        _imageService = imageService;
        _applicationUserService = applicationUserService;
    }

    public async Task<PageResult> OnGetAsync()
    {
        Galleries = await _imageService.GetGalleries();
        foreach (var gallery in Galleries)
        {
            CarouselModel.Content.Add(new CarouselContentModel()
            {
                ImageFileId = gallery.CoverImageId,
                ImageGalleryId = gallery.Id,
                Name = gallery.Name,
                Description = gallery.Description,
                Link = $"/{gallery.GalleryRootName}/{gallery.Name}"
            });
        }
        return Page();
    }
}