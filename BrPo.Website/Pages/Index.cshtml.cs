using BrPo.Website.Services.ContactForm.Models;
using BrPo.Website.Services.ContactForm.Services;
using BrPo.Website.Services.Email;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BrPo.Website.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        [BindProperty]
        public ContactModel ContactModel { get; set; }
        [BindProperty]
        public string Environment { get; set; }

        private readonly IContactService _contactService;
        private readonly IWebHostEnvironment _env;

        public IndexModel(
            ILogger<IndexModel> logger,
            IContactService contactService,
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _contactService = contactService;
            _env = env;
            Environment = env.EnvironmentName;
        }

        public IActionResult OnGet() {
            if (!Request.Cookies.ContainsKey("BrPoSession")){
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddMinutes(120);
                options.IsEssential = true;
                options.HttpOnly = true;
                Response.Cookies.Append("BrPoSession", _httpContextAccessor.HttpContext.Session.Id, options);
            }
            //return Redirect("/prints/order");
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
