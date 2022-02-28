using BrPo.Website.Services.ApplicationUser.Models;
using BrPo.Website.Services.ApplicationUser.Services;
using BrPo.Website.Services.ContactForm.Models;
using BrPo.Website.Services.ContactForm.Services;
using BrPo.Website.Services.Email;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BrPo.Website.Pages
{
    [BindProperties]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IApplicationUserService _applicationUserService;

        public ApplicationUser ApplicationUser { get; set; }
        public ContactModel ContactModel { get; set; }
        public string Environment { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IWebHostEnvironment env,
            IApplicationUserService applicationUserService)
        {
            _logger = logger;
            _env = env;
            _applicationUserService = applicationUserService;
            Environment = env.EnvironmentName;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser = await _applicationUserService.GetCurrentUserAsync(this.User);
            return Page();
        }
    }
}