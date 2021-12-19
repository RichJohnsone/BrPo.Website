using System;
using System.Threading.Tasks;
using BrPo.Website.Areas.Identity.Services;
using BrPo.Website.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BrPo.Website.Areas.Admin.Pages
{
    [BindProperties, Authorize(Roles = RolesConfig.Administrator)]
    public class EditRoleModel : PageModel
    {
        private readonly IAdminService _adminService;
        private RoleManager<IdentityRole> RoleManager { get; set; }

        public EditRoleModel(RoleManager<IdentityRole> roleManager, IAdminService adminService)
        {
            RoleManager = roleManager;
            _adminService = adminService;
        }

        public IdentityRole IdentityRole { get; set; }

        public void OnGet(string roleName)
        {
            IdentityRole = _adminService.GetRole(roleName);
        }

        public async Task<IActionResult> OnPost()
        {
            var role = _adminService.GetRole(IdentityRole.Id);
            try
            {
                await RoleManager.SetRoleNameAsync(role, IdentityRole.Name);
                return RedirectToPage("/RoleAdmin");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}