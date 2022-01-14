using BrPo.Website.Services.ShoppingBasket.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BrPo.Website.Areas.ShoppingBasket.Pages
{
    public class SuccessModel : PageModel
    {
        private readonly ILogger<SuccessModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IShoppingBasketService _shoppingBasketService;
        private string _userId;

        public SuccessModel(
            ILogger<SuccessModel> logger,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager,
            IShoppingBasketService shoppingBasketService)
        {
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _shoppingBasketService = shoppingBasketService;
        }

        public async Task<IActionResult> OnGet([FromQuery] string session_id)
        {
            //var sessionService = new SessionService();
            //Session session = sessionService.Get(session_id);

            //var customerService = new CustomerService();
            //Customer customer = customerService.Get(session.CustomerId);
            _userId = GetUserOrSessionId();
            return Page();
        }

        private string GetUserOrSessionId()
        {
            var principle = this.User;
            if (principle != null && principle.Identity.IsAuthenticated)
            {
                return _userManager.GetUserId(principle);
            }
            else
            {
                if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("BrPoSession", out var sessionId))
                    return sessionId.ToString();
                else
                {
                    throw new ApplicationException("Your session has expired");
                }
            }
        }
    }
}