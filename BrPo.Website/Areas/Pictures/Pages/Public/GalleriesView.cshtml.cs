using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BrPo.Website.Areas.Pictures.Pages.Public
{
    [BindProperties(SupportsGet = true)]
    public class GalleriesViewModel : PageModel
    {
        public string GalleryRootName { get; set; }

        public void OnGet()
        {
        }
    }
}