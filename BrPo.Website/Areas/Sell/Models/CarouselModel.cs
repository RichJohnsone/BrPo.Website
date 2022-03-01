using System.Collections.Generic;

namespace BrPo.Website.Areas.Sell.Models;

public class CarouselModel
{
    public string CarouselId { get; set; }
    public bool ShowIndicators { get; set; }
    public bool ShowContent { get; set; }
    public bool ShowNav { get; set; }
    public int ContentHeight { get; set; }
    public int IndicatorHeight { get; set; }
    public List<CarouselContentModel> Content { get; set; }

    public CarouselModel(List<CarouselContentModel> content = null)
    {
        ShowIndicators = true;
        ShowContent = true;
        ShowNav = true;
        ContentHeight = 300;
        IndicatorHeight = 30;
        Content = content == null ? new List<CarouselContentModel>() : content;
    }
}