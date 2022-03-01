using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BrPo.Website.Config;

public static class RolesConfig
{
    public const string User = "User";
    public const string Manager = "Manager";
    public const string Administrator = "Administrator";

    public static async Task InitialiseAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roleNames =
        {
            User,
            Manager,
            Administrator
        };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}