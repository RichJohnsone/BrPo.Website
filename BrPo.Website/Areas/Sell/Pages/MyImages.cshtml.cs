using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrPo.Website.Areas.Sell.Models;
using BrPo.Website.Services.ApplicationUser.Models;
using BrPo.Website.Services.ApplicationUser.Services;
using BrPo.Website.Services.Email;
using BrPo.Website.Services.Image.Models;
using BrPo.Website.Services.Image.Services;
using BrPo.Website.Services.Paper.Models;
using BrPo.Website.Services.Paper.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static BrPo.Website.Services.Paper.Models.Enums;

namespace BrPo.Website.Areas.Sell.Pages;

public class MyImagesModel : PageModel
{
    private readonly ILogger<MyImagesModel> _logger;
    private readonly IImageService _imageService;
    private readonly IApplicationUserService _applicationUserService;
    private readonly IPaperService _paperService;

    public ApplicationUser ApplicationUser { get; set; }
    public List<ImageFileModel> Files { get; set; } = new List<ImageFileModel>();
    public ImageFileModel SelectedFile { get; set; }
    public List<ImageGalleryItem> ImageGalleryItems { get; set; }
    public List<ImageGallery> ImageGalleries { get; set; }
    public int ImageGalleryId { get; set; }
    public ImageGallery SelectedGallery { get; set; }
    public List<PaperEnumItem> PaperSurfaces { get; set; }
    public List<PaperEnumItem> PaperTextures { get; set; }
    public string GalleryRootName { get; set; }
    public ImageScrollerModel ImageScroller { get; set; } = new ImageScrollerModel();
    public bool DisplayAsGrid { get; set; }

    public MyImagesModel(
        ILogger<MyImagesModel> logger,
        IImageService imageService,
        IApplicationUserService applicationUserService,
        IPaperService paperService)
    {
        _logger = logger;
        _imageService = imageService;
        _applicationUserService = applicationUserService;
        _paperService = paperService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        ApplicationUser = await _applicationUserService.GetCurrentUserAsync(this.User);
        SetPaperEnumLists();
        DisplayAsGrid = _applicationUserService.GetOrSetImagesViewCookieValue() == "slider" ? false : true;
        if (ApplicationUser.UserDetails?.GalleryRootName == null)
        {
            return Page();
        }
        else
        {
            GalleryRootName = ApplicationUser.UserDetails.GalleryRootName;
            ImageGalleryItems = await GetImageGalleryItemsAndFiles(ApplicationUser.Id);
            ImageGalleries = await _imageService.GetUserGalleriesAsync(ApplicationUser.Id);
        }
        return Page();
    }

    private async Task<List<ImageGalleryItem>> GetImageGalleryItemsAndFiles(Guid userId)
    {
        Files = _imageService.GetImages(userId.ToString());
        if (Files.Count > 0)
        {
            SelectedFile = Files[0];
        }
        var imageGalleryItems = await _imageService.GetGalleryItemsAsync(userId);
        foreach (var file in Files)
        {
            if (imageGalleryItems.Any(g => g.ImageFileId == file.Id)) continue;
            imageGalleryItems.Add(new ImageGalleryItem() { Id = 0, ImageFileId = file.Id, ImageFile = file });
        }
        return imageGalleryItems;
    }

    private void SetPaperEnumLists()
    {
        var lists = _paperService.GetPaperEnums();
        if (lists != null)
        {
            PaperSurfaces = lists[0];
            PaperTextures = lists[1];
        }
    }

    public PartialViewResult OnGetEditGalleryItemPartial(int galleryItemId, int fileId)
    {
        ApplicationUser = _applicationUserService.GetCurrentUserAsync(this.User).Result;
        var galleryItem = new ImageGalleryItem() { Id = 0, ImageFileId = fileId, Tags = new List<ImageTag>() };
        if (galleryItemId != 0) galleryItem = _imageService.GetGalleryItemAsync(galleryItemId, ApplicationUser.Id).Result;
        if (galleryItemId == 0)
        {
            galleryItem.ImageFile = _imageService.GetImageFileModelAsync(fileId).Result;
            CalculateSizes(ref galleryItem);
            galleryItem.MinPrice = 50;
            galleryItem.MaxPrice = 200;
            galleryItem.Galleries = new List<ImageGallery>();
        }
        else
        {
            galleryItem.Galleries = _imageService.GetGalleries(galleryItem.Id);
        }
        return Partial("EditGalleryItemPartial", galleryItem);
    }

    public PartialViewResult OnGetGalleryItemsImageScroller()
    {
        ApplicationUser = _applicationUserService.GetCurrentUserAsync(this.User).Result;
        if (!ApplicationUser.IsIdentityUser) Redirect("/Index");
        DisplayAsGrid = _applicationUserService.GetOrSetImagesViewCookieValue("slider") == "slider" ? false : true;
        var galleryItems = GetImageGalleryItemsAndFiles(ApplicationUser.Id).Result;
        var imageScroller = new ImageScrollerModel();
        foreach (var galleryItem in galleryItems)
        {
            imageScroller.Content.Add(new CarouselContentModel()
            {
                ImageFileId = galleryItem.ImageFile.Id,
                ImageGalleryItemId = galleryItem.Id,
                Name = galleryItem.Name,
                Description = galleryItem.Description,
                Link = $"/{ApplicationUser.UserDetails.GalleryRootName}/{galleryItem.Name}",
                LinkClass = "gallery-item-js"
            });
        }
        imageScroller.ImageScrollerId = "gallery-items-scroller";
        imageScroller.ShowButtons = true;
        return Partial("ImageScrollerPartial", imageScroller);
    }

    public PartialViewResult OnGetGalleryItemsGrid()
    {
        ApplicationUser = _applicationUserService.GetCurrentUserAsync(this.User).Result;
        if (!ApplicationUser.IsIdentityUser) Redirect("/Index");
        DisplayAsGrid = _applicationUserService.GetOrSetImagesViewCookieValue("grid") == "grid" ? true : false;
        var galleryItems = GetImageGalleryItemsAndFiles(ApplicationUser.Id).Result;
        return Partial("GalleryItemsGridPartial", galleryItems);
    }

    public async Task<IActionResult> OnGetGridAsync()
    {
        ApplicationUser = await _applicationUserService.GetCurrentUserAsync(this.User);
        var galleryItems = await GetImageGalleryItemsAndFiles(ApplicationUser.Id);
        return Partial("GalleryItemsGridPartial", galleryItems);
    }

    public async Task<IActionResult> OnPostDeleteGalleryItemAsync([FromQuery] int galleryItemId)
    {
        ApplicationUser = _applicationUserService.GetCurrentUserAsync(this.User).Result;
        try
        {
            await _imageService.DeleteGalleryItemAsync(galleryItemId, ApplicationUser.Id);
            return new OkResult();
        }
        catch (ApplicationException ae)
        {
            return BadRequest($"Error deleting gallery item: {ae.Message}");
        }
        catch (Exception e)
        {
            _logger.LogError("from GalleryItems.OnPostDeleteGalleryItemAsync", e);
            return BadRequest($"Error deleting gallery item: {e.Message}");
        }
    }

    public PartialViewResult OnGetImageTagsPartial(int galleryItemId)
    {
        List<ImageTag> imageTags = _imageService.GetImageTags(galleryItemId);
        return Partial("ImageTagsPartial", imageTags);
    }

    public async Task<IActionResult> OnPostCreateTagAsync([FromBody] CreateTagRequest request)
    {
        // check user signed in
        if (request == null) return BadRequest("No data");
        if (request.GalleryItemId == 0) return BadRequest("No galleryItemId specified");
        if (string.IsNullOrEmpty(request.Tag)
            || request.Tag.Length < 3
            || request.Tag.Length > 50) return BadRequest("Tag must be between 4 and 50 characters");
        /// TODO profanity check
        var imageTag = new ImageTag()
        {
            ImageGalleryItemId = request.GalleryItemId,
            Tag = request.Tag.Trim(),
            DateCreated = DateTime.UtcNow
        };
        try
        {
            await _imageService.CreateImageTag(imageTag);
            return new OkResult();
        }
        catch (Exception ex)
        {
            if (ex.Message == "Tag already exists")
            {
                return BadRequest(ex.Message);
            }
            _logger.LogError("from GalleryItems.OnPostCreateTagAsync", ex);
            return BadRequest($"Error creating tag: {ex.Message}");
        }
    }

    public async Task<IActionResult> OnPostDeleteTagAsync(int imageTagId)
    {
        // check user signed in
        await _imageService.DeleteImageTag(imageTagId);
        return new OkResult();
    }

    public PartialViewResult OnGetGalleryChipsPartial(int galleryItemId)
    {
        List<ImageGallery> galleries = _imageService.GetGalleries(galleryItemId);
        return Partial("GalleryChipsPartial", galleries);
    }

    public async Task<IActionResult> OnPostAddToGalleryAsync([FromBody] GalleryChangeRequest request)
    {
        // check user signed in
        if (request == null) return BadRequest("No data");
        if (request.GalleryItemId == 0) return BadRequest("No galleryItemId specified");
        if (request.GalleryId == 0) return BadRequest("No galleryId specified");
        var galleryContent = new ImageGalleryContent()
        {
            ImageGalleryItemId = request.GalleryItemId,
            ImageGalleryId = request.GalleryId,
            DateCreated = DateTime.UtcNow
        };
        try
        {
            await _imageService.AddToGallery(galleryContent);
            return new OkResult();
        }
        catch (Exception ex)
        {
            if (ex.Message == "Gallery entry already exists")
            {
                return BadRequest(ex.Message);
            }
            _logger.LogError("from GalleryItems.OnPostAddToGalleryAsync", ex);
            return BadRequest($"Error creating gallery content: {ex.Message}");
        }
    }

    public async Task<IActionResult> OnPostRemoveFromGalleryAsync([FromBody] GalleryChangeRequest request)
    {
        // check user signed in
        await _imageService.RemoveFromGallery(request.GalleryId, request.GalleryItemId);
        return new OkResult();
    }

    private void CalculateSizes(ref ImageGalleryItem galleryItem)
    {
        var orientation = galleryItem.ImageFile.Height > galleryItem.ImageFile.Width ? "portrait" : "landscape";
        var photoFatness = (Decimal)galleryItem.ImageFile.Width / galleryItem.ImageFile.Height;
        Decimal a4DimMax = 297, a4DimMin = 210, rollDimMin = 432, rollDimMax = 2000;
        Decimal paperFatness;
        Decimal mmHeightMin, mmHeightMax, mmWidthMin, mmWidthMax;
        Decimal scaleRatio = 1;
        // calculate min values
        if (orientation == "portrait")
        {
            paperFatness = a4DimMin / a4DimMax;
        }
        else
        {
            paperFatness = a4DimMax / a4DimMin;
        }
        if (photoFatness > paperFatness)
        {
            scaleRatio = (Decimal)galleryItem.ImageFile.Height / galleryItem.ImageFile.Width;
            mmWidthMin = a4DimMin;
            mmHeightMin = Math.Round((mmWidthMin * scaleRatio), 0);
        }
        else
        {
            scaleRatio = (Decimal)galleryItem.ImageFile.Width / galleryItem.ImageFile.Height;
            mmHeightMin = a4DimMin;
            mmWidthMin = Math.Round((mmHeightMin * scaleRatio), 0);
        }
        // calculate max values
        if (orientation == "portrait")
        {
            paperFatness = rollDimMin / rollDimMax;
        }
        else
        {
            paperFatness = rollDimMax / rollDimMin;
        }
        if (photoFatness > paperFatness)
        {
            scaleRatio = (Decimal)galleryItem.ImageFile.Height / galleryItem.ImageFile.Width;
            mmWidthMax = rollDimMin;
            mmHeightMax = Math.Round((mmWidthMax * scaleRatio), 0);
        }
        else
        {
            scaleRatio = (Decimal)galleryItem.ImageFile.Width / galleryItem.ImageFile.Height;
            mmHeightMax = rollDimMin;
            mmWidthMax = Math.Round((mmHeightMax * scaleRatio), 0);
        }
        galleryItem.MinHeight = (int)mmHeightMin;
        galleryItem.MaxHeight = (int)mmHeightMax;
        galleryItem.MinWidth = (int)mmWidthMin;
        galleryItem.MaxWidth = (int)mmWidthMax;
    }

    public async Task<IActionResult> OnPostSaveGalleryItemAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        ApplicationUser = _applicationUserService.GetCurrentUserAsync(this.User).Result;
        // read form
        var formVars = this.Request.Form;
        var isNew = false;
        var galleryItem = new ImageGalleryItem();
        try
        {
            galleryItem.Id = formVars["Id"].ToString().ToInt();
            galleryItem.Name = formVars["Name"].ToString();
            if (string.IsNullOrEmpty(galleryItem.Name)) return BadRequest("No name specified in request");
            galleryItem.ImageFileId = formVars["ImageFileId"].ToString().ToInt();
            galleryItem.MinHeight = formVars["MinHeight"].ToString().ToInt();
            galleryItem.MaxHeight = formVars["MaxHeight"].ToString().ToInt();
            galleryItem.MinWidth = formVars["MinWidth"].ToString().ToInt();
            galleryItem.MaxWidth = formVars["MaxWidth"].ToString().ToInt();
            galleryItem.MinPrice = formVars["MinPrice"].ToString().ToInt();
            galleryItem.MaxPrice = formVars["MaxPrice"].ToString().ToInt();
            galleryItem.IsActive = true;
            galleryItem.DateUpdated = DateTime.UtcNow;
            galleryItem.Description = formVars["Description"].ToString();
            galleryItem.UserId = ApplicationUser.Id;
            galleryItem.PaperSurface = GetPaperSurfaceByValue(formVars["SelectedPaperSurfaceId"].ToString().ToInt());
            galleryItem.PaperTexture = GetPaperTextureByValue(formVars["SelectedPaperTextureId"].ToString().ToInt());
            isNew = galleryItem.Id == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError("from GalleryItems.OnPostSaveGalleryItemAsync.1", ex);
            return BadRequest($"Error reading form values: {ex.ToMessageString()}");
        }
        try
        {
            await _imageService.AddOrUpdateGalleryItem(galleryItem);
            if (isNew) await AddToGallery(galleryItem.Id, formVars["ImageGalleryId"].ToString().ToInt());
            galleryItem.Galleries = _imageService.GetGalleries(galleryItem.Id);
            return new JsonResult(JsonConvert.SerializeObject(galleryItem));
        }
        catch (Exception ex)
        {
            _logger.LogError("from GalleryItems.OnPostSaveGalleryItemAsync.2", ex);
            return BadRequest($"Error saving form values: {ex.ToMessageString()}");
        }
    }

    private async Task AddToGallery(int galleryItemId, int galleryId)
    {
        var galleryContent = new ImageGalleryContent()
        {
            ImageGalleryItemId = galleryItemId,
            ImageGalleryId = galleryId,
            DateCreated = DateTime.UtcNow
        };
        await _imageService.AddToGallery(galleryContent);
    }

    private PaperSurface GetPaperSurfaceByValue(int value)
    {
        switch (value)
        {
            case 0: return PaperSurface.Any;
            case 1: return PaperSurface.Gloss;
            case 2: return PaperSurface.Matt;
            case 3: return PaperSurface.MetallicGloss;
            case 4: return PaperSurface.Velvet;
            default: return PaperSurface.Any;
        }
    }

    private PaperTexture GetPaperTextureByValue(int value)
    {
        switch (value)
        {
            case 0: return PaperTexture.Any;
            case 1: return PaperTexture.Smooth;
            case 2: return PaperTexture.Pearl;
            case 3: return PaperTexture.Silk;
            case 4: return PaperTexture.Lustre;
            case 5: return PaperTexture.Textured;
            default: return PaperTexture.Any;
        }
    }
}