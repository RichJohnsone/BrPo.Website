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
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public ContactModel ContactModel { get; set; }
        [BindProperty]
        public string Environment { get; set; }

        private readonly IContactService _contactService;
        private readonly IWebHostEnvironment _env;

        public IndexModel(
            ILogger<IndexModel> logger,
            IContactService contactService,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _contactService = contactService;
            _env = env;
            Environment = env.EnvironmentName;
        }

        public void OnGet() {}

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
