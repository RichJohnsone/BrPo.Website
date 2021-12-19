using System.Collections.Generic;
using System.Linq;
using BrPo.Website.Data;
using Microsoft.AspNetCore.Identity;

namespace BrPo.Website.Areas.Identity.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private static ApplicationDbContext _applicationDbContext;
        public AdminService(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
        }
        //public List<ApplicationUser> GetUsers()
        //{
        //    return _applicationDbContext.Users.ToList();
        //}

        //public void UpdateUser(ApplicationUser user)
        //{
        //    _applicationDbContext.Users.Update(user);
        //    _applicationDbContext.SaveChanges();
        //}

        public List<IdentityRole> GetRoles()
        {
            return _applicationDbContext.Roles.ToList();
        }

        public IdentityRole GetRole(string roleName)
        {
            return string.IsNullOrEmpty(roleName) ? null : _applicationDbContext.Roles.Single(x => x.Name == roleName);
        }

        //public async Task<IEnumerable<string>> GetUserEmailsInRoleAsync(string roleName)
        //{
        //    var users = await _userManager.GetUsersInRoleAsync(roleName);
        //    return users.Select(x => x.Email);
        //}

        public List<string> GetExistingUserRoles(IdentityUser user)
        {
            var roleIds = (from c in _applicationDbContext.UserRoles where c.UserId == user.Id select c.RoleId).ToList();
            var roles = (from c in _applicationDbContext.Roles where roleIds.Contains(c.Id) select c.Name).ToList();
            return roles;
        }

        //public ApplicationUser GetUser(string email)
        //{
        //    return _applicationDbContext.Users.AsNoTracking().Single(x => x.Email == email);
        //}

        //public void RemoveUser(string email)
        //{
        //    var user = _applicationDbContext.Users.Single(x => x.Email == email);
        //    _applicationDbContext.Users.Remove(user);
        //    _applicationDbContext.SaveChanges();
        //}

        public void RemoveRole(string roleName)
        {
            var role = _applicationDbContext.Roles.Single(x => x.Name == roleName);
            _applicationDbContext.Roles.Remove(role);
            _applicationDbContext.SaveChanges();
        }
    }
}
