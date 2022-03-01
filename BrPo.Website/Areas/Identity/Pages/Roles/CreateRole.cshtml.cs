using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BrPo.Website.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BrPo.Website.Areas.Identity.Pages.Roles;

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
        var role = new IdentityRole { Name = Name };
        await _roleManager.CreateAsync(role);
        return RedirectToPage("RoleAdmin");
    }
}