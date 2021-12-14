using BrPo.Website.Services.ContactForm.Models;
using BrPo.Website.Services.Image.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BrPo.Website.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<ImageFileModel> ImageFiles { get; set; }
    }
}