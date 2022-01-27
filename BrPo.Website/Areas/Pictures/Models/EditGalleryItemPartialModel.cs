using BrPo.Website.Services.Image.Models;

namespace BrPo.Website.Areas.Pictures.Models
{
    public class EditGalleryItemPartialModel
    {
        public ImageGalleryItem ImageGalleryItem { get; set; }
        public ImageFileModel ImageFile { get; set; }
    }
}