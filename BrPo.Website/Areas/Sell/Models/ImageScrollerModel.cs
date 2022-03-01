using System.Collections.Generic;

namespace BrPo.Website.Areas.Sell.Models;

public class ImageScrollerModel
{
    public string ImageScrollerId { get; set; }
    public int ContentHeight { get; set; }
    public bool ShowName { get; set; }
    public bool ShowButtons { get; set; }
    public List<CarouselContentModel> Content { get; set; }

    public ImageScrollerModel(List<CarouselContentModel> content = null)
    {
        ContentHeight = 100;
        ShowButtons = false;
        ShowName = true;
        Content = content == null ? new List<CarouselContentModel>() : content;
    }
}