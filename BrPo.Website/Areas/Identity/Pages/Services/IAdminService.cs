using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BrPo.Website.Areas.Identity.Pages.Services;

public interface IAdminService
{
    //void RemoveUser(string email);
    //List<ApplicationUser> GetUsers();
    //ApplicationUser GetUser(string email);
    //void UpdateUser(ApplicationUser user);

    List<IdentityRole> GetRoles();

    void RemoveRole(string roleName);

    IdentityRole GetRole(string roleName);

    List<string> GetExistingUserRoles(IdentityUser user);

    //Task<IEnumerable<string>> GetUserEmailsInRoleAsync(string roleName);
}