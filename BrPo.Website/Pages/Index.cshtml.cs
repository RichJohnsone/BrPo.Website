using BrPo.Website.Services.ContactForm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BrPo.Website.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public ContactModel ContactModel { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet() {}

        public async Task<IActionResult> OnPostSaveFormAsync()
        {
            await Task.Delay(20);
            return Content(ContactModel.Name);
        }
    }
}
