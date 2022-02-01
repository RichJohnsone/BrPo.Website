using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BrPo.Website.Areas.Pictures.Pages.Public
{
    [BindProperties(SupportsGet = true)]
    public class ImageViewModel : PageModel
    {
        public string GalleryRootName { get; set; }

        public string GalleryName { get; set; }

        public string ImageName { get; set; }

        public void OnGet()
        {
        }
    }
}