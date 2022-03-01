using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BrPo.Website.Areas.Sell.Pages.Public;

[BindProperties(SupportsGet = true)]
public class GalleriesViewModel : PageModel
{
    public string GalleryRootName { get; set; }

    public void OnGet()
    {
    }
}