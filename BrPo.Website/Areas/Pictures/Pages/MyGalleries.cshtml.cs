using BrPo.Website.Areas.Pictures.Models;
using BrPo.Website.Services.ApplicationUser.Models;
using BrPo.Website.Services.ApplicationUser.Services;
using BrPo.Website.Services.Email;
using BrPo.Website.Services.Image.Models;
using BrPo.Website.Services.Image.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrPo.Website.Areas.Pictures.Pages
{
    public class MyGalleriesModel : PageModel
    {
        private readonly ILogger<GalleryItemsModel> _logger;
        private readonly IImageService _imageService;
        private readonly IApplicationUserService _applicationUserService;

        public ApplicationUser ApplicationUser { get; set; }
        private List<ImageGallery> Galleries { get; set; }
        private ImageGallery SelectedGallery { get; set; }
        public CarouselModel CarouselModel { get; set; } = new CarouselModel();

        public ImageScrollerModel ImageScroller { get; set; } = new ImageScrollerModel();
        public bool DisplayAsGrid { get; set; }

        public MyGalleriesModel(
            ILogger<GalleryItemsModel> logger,
            IImageService imageService,
            IApplicationUserService applicationUserService)
        {
            _logger = logger;
            _imageService = imageService;
            _applicationUserService = applicationUserService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser = await _applicationUserService.GetCurrentUserAsync(this.User);
            if (!ApplicationUser.IsIdentityUser) Redirect("/Index");
            DisplayAsGrid = _applicationUserService.GetOrSetGalleriesViewCookieValue() == "slider" ? false : true;
            return Page();
        }

        public PartialViewResult OnGetEditGalleryPartial(int galleryId)
        {
            ApplicationUser = _applicationUserService.GetCurrentUserAsync(this.User).Result;
            if (!ApplicationUser.IsIdentityUser) Redirect("/Index");
            var gallery = new ImageGallery() { Id = 0, Content = new List<ImageGalleryContent>() };
            if (galleryId != 0) gallery = _imageService.GetGalleryAsync(galleryId, ApplicationUser.Id).Result;
            if (galleryId == 0)
            {
            }
            else
            {
            }
            return Partial("EditGalleryPartial", gallery);
        }

        public PartialViewResult OnGetGalleryImageScroller()
        {
            ApplicationUser = _applicationUserService.GetCurrentUserAsync(this.User).Result;
            if (!ApplicationUser.IsIdentityUser) Redirect("/Index");
            DisplayAsGrid = _applicationUserService.GetOrSetGalleriesViewCookieValue("slider") == "slider" ? false : true;
            var galleries = _imageService.GetGalleries(ApplicationUser.Id);
            var imageScroller = new ImageScrollerModel();
            foreach (var gallery in galleries)
            {
                imageScroller.Content.Add(new CarouselContentModel()
                {
                    ImageFileId = gallery.CoverImageId,
                    ImageGalleryId = gallery.Id,
                    Name = gallery.Name,
                    Description = gallery.Description,
                    Link = $"/{gallery.GalleryRootName}/{gallery.Name}",
                    LinkClass = "gallery-js"
                });
            }
            imageScroller.ImageScrollerId = "galleries-scroller";
            imageScroller.ShowButtons = true;
            return Partial("ImageScrollerPartial", imageScroller);
        }

        public PartialViewResult OnGetGalleryGrid()
        {
            ApplicationUser = _applicationUserService.GetCurrentUserAsync(this.User).Result;
            if (!ApplicationUser.IsIdentityUser) Redirect("/Index");
            DisplayAsGrid = _applicationUserService.GetOrSetGalleriesViewCookieValue("grid") == "grid" ? true : false;
            var galleries = _imageService.GetGalleries(ApplicationUser.Id);
            return Partial("GalleryGridPartial", galleries);
        }

        public async Task<IActionResult> OnGetGridAsync()
        {
            ApplicationUser = await _applicationUserService.GetCurrentUserAsync(this.User);
            var galleries = _imageService.GetGalleries(ApplicationUser.Id);
            return Partial("GalleryGridPartial", galleries);
        }

        public PartialViewResult OnGetGalleryContent(int galleryId)
        {
            ApplicationUser = _applicationUserService.GetCurrentUserAsync(this.User).Result;
            if (!ApplicationUser.IsIdentityUser) Redirect("/Index");
            List<ImageGalleryContent> contents = _imageService.GetGalleryContents(galleryId);
            var gallery = _imageService.GetGalleryAsync(galleryId, ApplicationUser.Id).Result;
            var imageScroller = new ImageScrollerModel();
            foreach (var content in contents)
            {
                imageScroller.Content.Add(new CarouselContentModel()
                {
                    ImageFileId = content.ImageGalleryItem.ImageFile.Id,
                    ImageGalleryId = content.ImageGalleryId,
                    ImageGalleryItemId = content.ImageGalleryItemId,
                    Name = content.ImageGalleryItem.Name,
                    Description = content.ImageGalleryItem.Description,
                    Link = "", // $"/{ApplicationUser.UserDetails.GalleryRootName}/{gallery.Name}/{content.ImageGalleryItem.Name}"
                    LinkClass = "content-js"
                });
            }
            imageScroller.ImageScrollerId = "content-scroller";
            return Partial("ImageScrollerPartial", imageScroller);
        }

        public async Task<IActionResult> OnPostSaveGalleryAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            ApplicationUser = await _applicationUserService.GetCurrentUserAsync(this.User);
            if (!ApplicationUser.IsIdentityUser) Redirect("/Index");
            // read form
            var formVars = this.Request.Form;
            var gallery = new ImageGallery();
            try
            {
                gallery.UserId = ApplicationUser.Id;
                gallery.DateUpdated = DateTime.UtcNow;
                gallery.Id = formVars["Id"].ToString().ToInt();
                gallery.Name = formVars["Name"].ToString();
                gallery.Description = formVars["Description"].ToString();
                gallery.GalleryRootName = ApplicationUser.UserDetails.GalleryRootName;
                if (string.IsNullOrEmpty(gallery.Name)) return BadRequest("No name specified in request");
                gallery.CoverImageId = formVars["CoverImageId"].ToString().ToInt();
            }
            catch (Exception ex)
            {
                _logger.LogError("from MyGalleries.OnPostSaveGalleryAsync.1", ex);
                return BadRequest($"Error reading form values: {ex.ToMessageString()}");
            }
            try
            {
                await _imageService.AddOrUpdateGallery(gallery, ApplicationUser.Id);
                return new JsonResult(JsonConvert.SerializeObject(gallery));
            }
            catch (Exception ex)
            {
                _logger.LogError("from MyGalleries.OnPostSaveGalleryAsync.2", ex);
                return BadRequest($"Error saving form values: {ex.ToMessageString()}");
            }
        }

        public async Task<IActionResult> OnPostDeleteGalleryAsync()
        {
            ApplicationUser = _applicationUserService.GetCurrentUserAsync(this.User).Result;
            if (!ApplicationUser.IsIdentityUser) Redirect("/Index");
            return new OkResult();
        }
    }
}