using Newtonsoft.Json;

namespace BrPo.Website.Areas.Pictures.Models
{
    public class GalleryChangeRequest
    {
        [JsonProperty("galleryItemId")]
        public int GalleryItemId { get; set; }

        [JsonProperty("galleryId")]
        public int GalleryId { get; set; }
    }
}