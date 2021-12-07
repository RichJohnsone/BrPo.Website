using BrPo.Website.Services.ContactForm.Models;
using BrPo.Website.Services.ContactForm.Services;
using BrPo.Website.Services.Email;
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

        private readonly IContactService _contactService;

        public IndexModel(
            ILogger<IndexModel> logger,
            IContactService contactService)
        {
            _logger = logger;
            _contactService = contactService;
        }

        public void OnGet() {}

        public async Task<IActionResult> OnPostSaveFormAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // do some serverside validation
            if (!ContactModel.Email.IsValidEmailAddress())
            {
                return BadRequest("Email address is not valid");
            }
            var existingContact = _contactService.Find(ContactModel);
            if (existingContact == null)
                await _contactService.Save(ContactModel);
            else
                return BadRequest("This request has been saved already");
            return Content(ContactModel.Name);
        }
    }
}
