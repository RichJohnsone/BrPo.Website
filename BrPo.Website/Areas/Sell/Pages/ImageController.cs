using BrPo.Website.Services.Image.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrPo.Website.Areas.Sell.Pages;

[Route("api/[controller]")]
[ApiController]
public class ImageController : Controller
{
    private readonly IImageService _imageService;

    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }

    // Get api/images
    public IActionResult Index(int imageId, int height, int width)
    {
        if (imageId == 0) return null;
        return File(_imageService.GetImageAsync(imageId, height, width).Result, "image/jpeg");
    }
}