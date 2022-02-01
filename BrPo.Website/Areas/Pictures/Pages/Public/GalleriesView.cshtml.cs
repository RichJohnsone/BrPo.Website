using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BrPo.Website.Areas.Pictures.Pages.Public
{
    public class GalleriesViewModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string GalleryRootName { get; set; }

        public void OnGet()
        {
        }
    }
}