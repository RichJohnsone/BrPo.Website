using BrPo.Website.Services.ContactForm.Models;
using BrPo.Website.Services.ContactForm.Services;
using BrPo.Website.Services.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BrPo.Website.Pages
{
    public class ContactModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IContactService _contactService;
        public Contact Contact { get; set; }

        public ContactModel(
            ILogger<IndexModel> logger,
            IContactService contactService)
        {
            _logger = logger;
            _contactService = contactService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostSaveFormAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (!Contact.Email.IsValidEmailAddress())
            {
                return BadRequest("Email address is not valid");
            }
            var existingContact = _contactService.Find(Contact);
            if (existingContact == null)
            {
                Contact.DateCreated = System.DateTime.UtcNow;
                await _contactService.Save(Contact);
                await _contactService.Email(Contact);
            }
            else
                return BadRequest("This request has been saved already");
            return Content(Contact.Name);
        }
    }
}