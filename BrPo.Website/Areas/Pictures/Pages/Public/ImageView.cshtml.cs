using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BrPo.Website.Areas.Pictures.Pages.Public
{
    public class ImageViewModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string GalleryRootName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string GalleryName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ImageName { get; set; }

        public void OnGet()
        {
        }
    }
}