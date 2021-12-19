using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BrPo.Website.Config;

namespace BrPo.Website.Areas.Admin.Pages
{
    [BindProperties, Authorize(Roles = RolesConfig.Administrator)]
    public class CreateRoleModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateRoleModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [Required]
        public string Name { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            var role = new IdentityRole{Name = Name};
            await _roleManager.CreateAsync(role);
            return RedirectToPage("RoleAdmin");
        }
    }
}