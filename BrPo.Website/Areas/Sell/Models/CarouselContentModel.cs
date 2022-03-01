using System.Collections.Generic;
using BrPo.Website.Services.Image.Models;

namespace BrPo.Website.Areas.Sell.Models;

public class CarouselContentModel
{
    public int ImageFileId { get; set; }
    public int ImageGalleryId { get; set; }
    public int ImageGalleryItemId { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public string Link { get; set; }
    public string LinkClass { get; set; }
    public string LinkTitle { get; set; }
    public List<ImageTag> Tags { get; set; }
    public int ViewHeight { get; set; }
    public int ViewWidth { get; set; }
    public bool IsPortrait { get; set; }
}