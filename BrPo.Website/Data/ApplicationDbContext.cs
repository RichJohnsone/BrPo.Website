using BrPo.Website.Services.ContactForm.Models;
using BrPo.Website.Services.Image.Models;
using BrPo.Website.Services.Paper.Models;
using BrPo.Website.Services.ShoppingBasket.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BrPo.Website.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set default precision to decimal property
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(10, 2)");
            }
        }

        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<ImageFileModel> ImageFiles { get; set; }
        public DbSet<PaperModel> Papers { get; set; }
        public DbSet<BasketItem> basketItems { get; set; }
        public DbSet<PrintOrder> PrintOrders { get; set; }
    }
}