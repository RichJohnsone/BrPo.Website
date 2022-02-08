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

        string GetOrSetImagesViewCookieValue(string value = null);

        string GetOrSetGalleriesViewCookieValue(string value = null);
    }

    public class ApplicationUserService : IApplicationUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ApplicationUserService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        internal const String BrPoSessionCookieKey = "BrPoSession";
        internal const int BrPoSessionCookieExpiry = 120;
        internal const String BrPoImagesViewCookieKey = "BrPoImagesView";
        internal const int BrPoImagesViewCookieExpiry = 525600;
        internal const String BrPoGalleriesViewCookieKey = "BrPoGalleriesView";
        internal const int BrPoGalleriesViewCookieExpiry = 525600;

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
                if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(BrPoSessionCookieKey, out var sessionId))
                {
                    Guid.TryParse(sessionId, out var guid);
                    applicationUser.Id = guid;
                    CreateCookie(BrPoSessionCookieKey, sessionId, BrPoSessionCookieExpiry);
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

        public string GetOrSetImagesViewCookieValue(string value = null)
        {
            var cookieValue = value;
            if (_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(BrPoImagesViewCookieKey))
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[BrPoImagesViewCookieKey].ToString();
                if (value == null) return cookieValue;
                if (cookieValue == value) return cookieValue;
                cookieValue = value;
            }
            else
            {
                if (value == null) cookieValue = "slider";
            }
            CreateCookie(BrPoImagesViewCookieKey, cookieValue, BrPoImagesViewCookieExpiry);
            return cookieValue;
        }

        public string GetOrSetGalleriesViewCookieValue(string value = null)
        {
            var cookieValue = value;
            if (_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(BrPoGalleriesViewCookieKey))
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[BrPoGalleriesViewCookieKey].ToString();
                if (value == null) return cookieValue;
                if (cookieValue == value) return cookieValue;
                cookieValue = value;
            }
            else
            {
                if (value == null) cookieValue = "slider";
            }
            CreateCookie(BrPoGalleriesViewCookieKey, cookieValue, BrPoGalleriesViewCookieExpiry);
            return cookieValue;
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
                    CreateCookie(BrPoSessionCookieKey, newId.ToString(), BrPoSessionCookieExpiry);
                    return guestUser;
                }
            }
        }

        private void CreateCookie(string key, string value, int expiryInMinutes)
        {
            if (_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(key))
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(key);
            }
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddMinutes(expiryInMinutes);
            options.MaxAge = TimeSpan.FromMinutes(expiryInMinutes);
            options.IsEssential = true;
            options.HttpOnly = true;
            options.Secure = true;
            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, options);
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