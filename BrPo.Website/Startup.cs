using BrPo.Website.Data;
using BrPo.Website.Services.ContactForm.Services;
using BrPo.Website.Services.Email;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace BrPo.Website
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        // Enables the SessionId cookie without non-essential cookie consent.
        public static bool SessionIdCookieIsEssential { get; } = true;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"), serverVersion: ServerVersion.AutoDetect(Configuration.GetConnectionString("DefaultConnection")),
                mySqlOptionsAction: options => { options.EnableRetryOnFailure(); }
            ));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, IdentityEmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/Uploads");
            })
                .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; })
                .AddCookieTempDataProvider(options => { options.Cookie.IsEssential = true; });
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient<BrPo.Website.Services.Email.IEmailSender, EmailSender>();

            services.AddAntiforgery(option =>
            {
                option.HeaderName = "XSRF-TOKEN";
                option.SuppressXFrameOptionsHeader = false;
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "BrPoAppCookie";
                options.AccessDeniedPath = "/AccessDenied";
                options.SlidingExpiration = true;
                options.ReturnUrlParameter = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromHours(2);
                //options.Cookie.Expiration = TimeSpan.FromHours(2);
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                // Sets the display of the Cookie Consent banner (/Pages/Shared/_CookieConsentPartial.cshtml).
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
