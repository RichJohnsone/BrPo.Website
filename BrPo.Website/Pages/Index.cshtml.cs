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
        private readonly IContactService _contactService;
        private readonly IWebHostEnvironment _env;
        private readonly IApplicationUserService _applicationUserService;

        public ApplicationUser ApplicationUser { get; set; }
        public ContactModel ContactModel { get; set; }
        public string Environment { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IContactService contactService,
            IWebHostEnvironment env,
            IApplicationUserService applicationUserService)
        {
            _logger = logger;
            _contactService = contactService;
            _env = env;
            _applicationUserService = applicationUserService;
            Environment = env.EnvironmentName;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser = await _applicationUserService.GetCurrentUserAsync(this.User);
            return Page();
        }

        public async Task<IActionResult> OnPostSaveFormAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (!ContactModel.Email.IsValidEmailAddress())
            {
                return BadRequest("Email address is not valid");
            }
            var existingContact = _contactService.Find(ContactModel);
            if (existingContact == null)
            {
                ContactModel.DateCreated = System.DateTime.UtcNow;
                await _contactService.Save(ContactModel);
                await _contactService.Email(ContactModel);
            }
            else
                return BadRequest("This request has been saved already");
            return Content(ContactModel.Name);
        }
    }
}