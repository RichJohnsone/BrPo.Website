using BrPo.Website.Services.Image.Models;
using System.Collections.Generic;

namespace BrPo.Website.Areas.Pictures.Models
{
    public class CarouselContentModel
    {
        public int ImageFileId { get; set; }
        public int ImageGalleryId { get; set; }
        public int ImageGalleryItemId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public List<ImageTag> Tags { get; set; }
        public string LinkClass { get; set; }
    }
}