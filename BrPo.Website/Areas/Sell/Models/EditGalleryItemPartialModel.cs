using BrPo.Website.Services.Image.Models;

namespace BrPo.Website.Areas.Sell.Models;

public class EditGalleryItemPartialModel
{
    public ImageGalleryItem ImageGalleryItem { get; set; }
    public ImageFileModel ImageFile { get; set; }
}