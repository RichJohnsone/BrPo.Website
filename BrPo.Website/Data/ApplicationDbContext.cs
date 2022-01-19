using BrPo.Website.Services.ApplicationUser.Models;
using BrPo.Website.Services.ContactForm.Models;
using BrPo.Website.Services.Image.Models;
using BrPo.Website.Services.Paper.Models;
using BrPo.Website.Services.ShoppingBasket.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;

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
            this.SeedUsers(modelBuilder);
            this.SeedRoles(modelBuilder);
            this.SeedUserRoles(modelBuilder);
            this.SeedPapers(modelBuilder);
        }

        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<ImageFileModel> ImageFiles { get; set; }
        public DbSet<PaperModel> Papers { get; set; }
        public DbSet<BasketItem> basketItems { get; set; }
        public DbSet<PrintOrderItem> PrintOrderItems { get; set; }
        public DbSet<PrintInvoiceItem> PrintInvoiceItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<GuestUserModel> GuestUsers { get; set; }
        public DbSet<UserDetailsModel> UserDetails { get; set; }

        private void SeedUsers(ModelBuilder builder)
        {
            IdentityUser user = new IdentityUser()
            {
                Id = "b74ddd14-6340-4840-95c2-db12554843e5",
                UserName = "BPAdmin",
                Email = "Info@brixtonPhotographic.com",
                LockoutEnabled = false,
                PhoneNumber = "07986215451"
            };
            PasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();
            passwordHasher.HashPassword(user, "BPAdm1n28*-");
            builder.Entity<IdentityUser>().HasData(user);
        }

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = "fab4fac1-c546-41de-aebc-a14da6895711", Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new IdentityRole() { Id = "c7b013f0-5201-4317-abd8-c211f91b7330", Name = "Manager", ConcurrencyStamp = "2", NormalizedName = "Manager" },
                new IdentityRole() { Id = "8e445865-a24d-4543-a6c6-9443d048cdb9", Name = "User", ConcurrencyStamp = "3", NormalizedName = "User" });
        }

        private void SeedUserRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { RoleId = "fab4fac1-c546-41de-aebc-a14da6895711", UserId = "b74ddd14-6340-4840-95c2-db12554843e5" }
                );
        }

        private void SeedPapers(ModelBuilder builder)
        {
            builder.Entity<PaperModel>().HasData(
                new PaperModel()
                {
                    Id = 1,
                    Name = "Ilford Galerie Prestige Smooth Gloss - 17 roll",
                    Description = "natural white smooth gloss surface",
                    GSMWeight = 310,
                    RollPaper = true,
                    DateCreated = System.DateTime.UtcNow,
                    RollWidth = 432,
                    CostPerMeter = 32,
                    ProductCode = "GPSGP12",
                    IsActive = true
                },
                new PaperModel()
                {
                    Id = 2,
                    Name = "Ilford Galerie Graphic Heavyweight Matt - 17 roll",
                    Description = "natural white smooth matte surface",
                    GSMWeight = 190,
                    RollPaper = true,
                    DateCreated = System.DateTime.UtcNow,
                    RollWidth = 432,
                    CostPerMeter = 30,
                    CostPerSheet = 0,
                    ProductCode = "IGXHWMP",
                    IsActive = true
                },
                new PaperModel()
                {
                    Id = 3,
                    Name = "Ilford Galerie Smooth Pearl - 17 roll",
                    Description = "natural white lightly pearled surface",
                    GSMWeight = 290,
                    RollPaper = true,
                    DateCreated = System.DateTime.UtcNow,
                    RollWidth = 432,
                    CostPerMeter = 35,
                    ProductCode = "IGSPP11",
                    IsActive = true
                },
                new PaperModel()
                {
                    Id = 4,
                    Name = "Olmec Photo Matt Archival - 17 roll",
                    Description = "ultra white smooth matte surface",
                    GSMWeight = 230,
                    RollPaper = true,
                    DateCreated = System.DateTime.UtcNow,
                    RollWidth = 432,
                    CostPerMeter = 30,
                    ProductCode = "OLM67R17",
                    IsActive = true
                },
                new PaperModel()
                {
                    Id = 5,
                    Name = "Matt Self-adhesive Poly-vinyl - 17 roll",
                    Description = "natural white smooth matte surface, grey backing for added opacity, easy-peel adhesive",
                    GSMWeight = 120,
                    RollPaper = true,
                    DateCreated = System.DateTime.UtcNow,
                    RollWidth = 432,
                    CostPerMeter = 45,
                    ProductCode = "M120V17",
                    IsActive = true
                },
                new PaperModel()
                {
                    Id = 6,
                    Name = "Olmec Photo Metallic Lustre A3",
                    Description = "neutral white smooth lustre mettalic surface",
                    GSMWeight = 260,
                    RollPaper = false,
                    DateCreated = System.DateTime.UtcNow,
                    CutSheetHeight = 420,
                    CutSheetWidth = 297,
                    CostPerSheet = 12,
                    ProductCode = "OLM72A3",
                    IsActive = true
                }
            );
        }
    }
}