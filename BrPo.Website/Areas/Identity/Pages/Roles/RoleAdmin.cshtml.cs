using System.Collections.Generic;
using BrPo.Website.Areas.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BrPo.Website.Areas.Admin.Pages
{
    [BindProperties]
    public class RoleAdminModel : PageModel
    {
        public List<IdentityRole> Roles { get; set; }
        private readonly IAdminService _adminService;

        public RoleAdminModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public void OnGet()
        {
            Roles = _adminService.GetRoles();
        }

        public IActionResult OnPostDeleteRole(string name)
        {
            var role = _adminService.GetRole(name);
            _adminService.RemoveRole(role.Name);
            return RedirectToPage("/RoleAdmin");
        }
    }
}