using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BrPo.Website.Services.ApplicationUser.Models;
using System;
using Microsoft.Extensions.Logging;
using BrPo.Website.Services.ApplicationUser.Services;

namespace BrPo.Website.Areas.Identity.Pages.Account.Manage;

[BindProperties]
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

    public UserDetailsModel UserDetailsModel { get; set; }

    public string StatusMessage { get; set; }

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
            UserDetailsModel.UserId = ApplicationUser.Id;
            UserDetailsModel.DateUpdated = DateTime.UtcNow;
            if (UserDetailsModel.Id != 0 && !string.IsNullOrEmpty(ApplicationUser.UserDetails?.GalleryRootName)) UserDetailsModel.GalleryRootName = ApplicationUser.UserDetails.GalleryRootName;
            await _applicationUserService.AddOrUpdateUserDetailsAsync(UserDetailsModel);
            StatusMessage = $"User details updated";
        }
        catch (Exception ex)
        {
            _logger.LogError("from EditUserDetails.OnPostAsync", ex);
            StatusMessage = $"Error saving user details: {ex.Message}";
        }

        return Page();
    }
}