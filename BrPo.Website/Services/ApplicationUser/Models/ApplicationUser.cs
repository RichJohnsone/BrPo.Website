using Microsoft.AspNetCore.Identity;
using System;

namespace BrPo.Website.Services.ApplicationUser.Models
{
    public class ApplicationUser
    {
        public Guid Id { get; set; }
        public IdentityUser IdentityUser { get; set; }
        public UserDetailsModel UserDetails { get; set; }
        public GuestUserModel GuestUser { get; set; }
        public bool IsIdentityUser { get; set; }
    }
}