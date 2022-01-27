using BrPo.Website.Services.ApplicationUser.Services;
using BrPo.Website.Services.Image.Models;
using BrPo.Website.Services.Image.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrPo.Website.Areas.Pictures.Pages
{
    [BindProperties]
    public class GalleriesModel : PageModel
    {
        private readonly ILogger<GalleryItemsModel> _logger;
        private readonly IImageService _imageService;
        private readonly IApplicationUserService _applicationUserService;

        public List<ImageGallery> Galleries { get; set; }

        public GalleriesModel(
            ILogger<GalleryItemsModel> logger,
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
            return Page();
        }
    }
}