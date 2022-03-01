using Newtonsoft.Json;

namespace BrPo.Website.Areas.Sell.Models;

public class CreateTagRequest
{
    [JsonProperty("galleryItemId")]
    public int GalleryItemId { get; set; }

    [JsonProperty("tag")]
    public string Tag { get; set; }
}