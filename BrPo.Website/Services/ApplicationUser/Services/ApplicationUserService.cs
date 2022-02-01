using BrPo.Website.Data;
using BrPo.Website.Services.ApplicationUser.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using BrPo.Website.Services.Email;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BrPo.Website.Services.ApplicationUser.Services
{
    public interface IApplicationUserService
    {
        Task<Models.ApplicationUser> GetCurrentUserAsync(ClaimsPrincipal user);

        Task CreateUserDetailsAsync(UserDetailsModel userDetailsModel);

        UserDetailsModel GetUserDetails(string userId);

        Task<string> GetGalleryRootName(Guid guid);

        Task AddOrUpdateUserDetailsAsync(UserDetailsModel userDetailsModel);
    }

    public class ApplicationUserService : IApplicationUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ApplicationUserService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        internal const String BrPoSessionCookieName = "BrPoSession";

        public ApplicationUserService(
            ApplicationDbContext applicationDbContext,
            IEmailSender emailSender,
            ILogger<ApplicationUserService> logger,
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _context = applicationDbContext;
            _emailSender = emailSender;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<Models.ApplicationUser> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            var identityUser = await _userManager.GetUserAsync(user);
            var applicationUser = new Models.ApplicationUser();
            if (user != null && user.Identity.IsAuthenticated)
            {
                Guid.TryParse(identityUser.Id, out var guid);
                applicationUser.Id = guid;
                applicationUser.IdentityUser = identityUser;
                applicationUser.IsIdentityUser = true;
                applicationUser.UserDetails = _context.UserDetails.First(u => u.UserId == guid) ?? null;
            }
            else
            {
                applicationUser.IsIdentityUser = false;
                if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(BrPoSessionCookieName, out var sessionId))
                {
                    Guid.TryParse(sessionId, out var guid);
                    applicationUser.Id = guid;
                    CreateCookie(sessionId);
                }
                else
                {
                    applicationUser.GuestUser = await GetNewGuestUser();
                    applicationUser.Id = applicationUser.GuestUser.Id;
                }
            }
            return applicationUser;
        }

        public async Task CreateUserDetailsAsync(UserDetailsModel userDetailsModel)
        {
            await Task.Delay(20);
            _context.UserDetails.Add(userDetailsModel);
            await _context.SaveChangesAsync();
        }

        public async Task AddOrUpdateUserDetailsAsync(UserDetailsModel userDetailsModel)
        {
            var entity = _context.UserDetails.Find(userDetailsModel.Id);
            if (userDetailsModel == null) return;
            if (userDetailsModel.Id == 0)
            {
                if (entity != null)
                    throw new ApplicationException("User details record already exists");
                userDetailsModel.DateCreated = (DateTime)userDetailsModel.DateUpdated;
                _context.UserDetails.Add(userDetailsModel);
            }
            else
            {
                if (entity != null)
                {
                    if (entity.UserId != userDetailsModel.UserId)
                        throw new ApplicationException("Attempt to update user details with incorrect userId");
                    var entry = _context.Entry(entity);
                    entry.CurrentValues.SetValues(userDetailsModel);
                }
            }
            await _context.SaveChangesAsync();
        }

        public UserDetailsModel GetUserDetails(string userId)
        {
            Guid.TryParse(userId, out var guid);
            var contact = _context.UserDetails.FirstOrDefault(u => u.UserId == guid);
            return contact;
        }

        public async Task<string> GetGalleryRootName(Guid guid)
        {
            return _context.UserDetails.FirstOrDefault(u => u.UserId == guid).GalleryRootName;
        }

        private async Task<GuestUserModel> GetNewGuestUser()
        {
            Guid newId;
            while (true)
            {
                newId = Guid.NewGuid();
                if (!GuestUserExists(newId) && !IdentityUserExists(newId.ToString()))
                {
                    var guestUser = new GuestUserModel(newId);
                    _context.GuestUsers.Add(guestUser);
                    await _context.SaveChangesAsync();
                    CreateCookie(newId.ToString());
                    return guestUser;
                }
            }
        }

        private void CreateCookie(string userId)
        {
            if (_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(BrPoSessionCookieName))
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(BrPoSessionCookieName);
            }
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddMinutes(120);
            options.MaxAge = TimeSpan.FromHours(2);
            options.IsEssential = true;
            options.HttpOnly = true;
            _httpContextAccessor.HttpContext.Response.Cookies.Append(BrPoSessionCookieName, userId, options);
        }

        private bool GuestUserExists(Guid userId)
        {
            return _context.GuestUsers.Any(g => g.Id == userId);
        }

        private bool IdentityUserExists(string userId)
        {
            return _userManager.Users.Any(u => u.Id == userId);
        }
    }
}