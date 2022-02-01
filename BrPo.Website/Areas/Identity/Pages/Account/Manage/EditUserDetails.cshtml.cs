using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BrPo.Website.Services.ApplicationUser.Models;
using System;
using Microsoft.Extensions.Logging;
using BrPo.Website.Services.ApplicationUser.Services;
using BrPo.Website.Services.Email;

namespace BrPo.Website.Areas.Identity.Pages.Account.Manage
{
    public class EditUserDetailsModel : PageModel
    {
        private readonly ILogger<EditUserDetailsModel> _logger;
        private readonly IApplicationUserService _applicationUserService;
        public ApplicationUser ApplicationUser { get; set; }

        public EditUserDetailsModel(
            ILogger<EditUserDetailsModel> logger,
            IApplicationUserService applicationUserService)
        {
            _logger = logger;
            _applicationUserService = applicationUserService;
        }

        [BindProperty]
        public UserDetailsModel UserDetailsModel { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser = await _applicationUserService.GetCurrentUserAsync(this.User);
            if (!ApplicationUser.IsIdentityUser)
            {
                return NotFound();
            }
            UserDetailsModel = ApplicationUser.UserDetails ?? new UserDetailsModel();
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ApplicationUser = await _applicationUserService.GetCurrentUserAsync(this.User);
            if (!ApplicationUser.IsIdentityUser)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                if (UserDetailsModel.Id == 0) UserDetailsModel.UserId = ApplicationUser.Id;
                UserDetailsModel.DateUpdated = DateTime.UtcNow;
                await _applicationUserService.AddOrUpdateUserDetailsAsync(UserDetailsModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("from EditUserDetails.OnPostAsync", ex);
                return BadRequest($"Error saving form values: {ex.ToMessageString()}");
            }

            return Page();
        }
    }
}