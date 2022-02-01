using AutoMapper;
using BrPo.Website.Areas.Identity.Services;
using BrPo.Website.Config;
using BrPo.Website.Data;
using BrPo.Website.Services.ApplicationUser.Services;
using BrPo.Website.Services.ContactForm.Services;
using BrPo.Website.Services.Email;
using BrPo.Website.Services.Image.Services;
using BrPo.Website.Services.Paper.Services;
using BrPo.Website.Services.ShoppingBasket.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Stripe;
using System;
using System.Collections.Generic;

namespace BrPo.Website
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // Enables the SessionId cookie without non-essential cookie consent.
        public static bool SessionIdCookieIsEssential { get; } = true;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
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
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddRazorPages(options =>
            {
            })
                .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; })
                .AddCookieTempDataProvider(options => { options.Cookie.IsEssential = true; });
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient<BrPo.Website.Services.Email.IEmailSender, EmailSender>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IAdminService, AdminService>();
            services.AddTransient<IPaperService, PaperService>();
            services.AddTransient<IShoppingBasketService, ShoppingBasketService>();
            services.AddTransient<IApplicationUserService, ApplicationUserService>();
            services.AddAntiforgery(option =>
            {
                option.HeaderName = "XSRF-TOKEN";
                option.SuppressXFrameOptionsHeader = false;
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(24);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "BrPoAppCookie";
                options.AccessDeniedPath = "/AccessDenied";
                options.SlidingExpiration = true;
                options.ReturnUrlParameter = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromHours(4);
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
            services.AddMvc();
            if (Environment.IsDevelopment())
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                });
            }
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            StripeConfiguration.ApiKey = Configuration["Stripe:ApiKey"];
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseStatusCodePagesWithReExecute("/Status/{0}");
                //app.UseExceptionHandler("/Status");
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Status/{0}");
                app.UseExceptionHandler("/Status");
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            //app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapRouteForExistingEndpoint("/{galleryrootname}/{galleryname}/{imagename}", "/Public/ImageView");
                endpoints.MapRouteForExistingEndpoint("/{galleryrootname}/{galleryname}", "/Public/GalleryView");
                endpoints.MapRouteForExistingEndpoint("/{galleryrootname}", "/Public/GalleriesView");
            });
            app.Use((context, next) =>
            {
                var currentEndPoint = context.GetEndpoint();
                var routes = context.Request.RouteValues;
                return next();
            });
        }
    }

    public static class EndpointBuilderExtensions
    {
        public static IEndpointConventionBuilder MapRouteForExistingEndpoint(
           this IEndpointRouteBuilder endpoints,
           string pattern,
           string existingEndpointDisplayName)
        {
            IEnumerable<string> httpMethods = new[] { "GET", "POST" };
            var eds = endpoints.DataSources.GetEnumerator();
            while (eds.MoveNext())
            {
                foreach (var ep in eds.Current.Endpoints)
                {
                    if (ep.DisplayName == existingEndpointDisplayName)
                    {
                        var builder = endpoints.Map(RoutePatternFactory.Parse(pattern), ep.RequestDelegate);
                        builder.WithDisplayName($"{pattern} HTTP: {string.Join(", ", httpMethods)}");
                        builder.WithMetadata(new HttpMethodMetadata(httpMethods));
                        return builder;
                    }
                }
            }

            return null;
        }
    }
}