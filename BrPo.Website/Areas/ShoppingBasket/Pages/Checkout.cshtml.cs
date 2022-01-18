using BrPo.Website.Services.ApplicationUser.Models;
using BrPo.Website.Services.ApplicationUser.Services;
using BrPo.Website.Services.ShoppingBasket.Models;
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
    [BindProperties]
    public class CheckoutModel : PageModel
    {
        private readonly ILogger<SuccessModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IShoppingBasketService _shoppingBasketService;
        private readonly IApplicationUserService _applicationUserService;

        public ApplicationUser ApplicationUser { get; set; }
        public Invoice Invoice { get; private set; }

        public CheckoutModel(
            ILogger<SuccessModel> logger,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager,
            IShoppingBasketService shoppingBasketService,
            IApplicationUserService applicationUserService)
        {
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _shoppingBasketService = shoppingBasketService;
            _applicationUserService = applicationUserService;
        }

        public async Task<IActionResult> OnGetAsync(int invoiceId)
        {
            ApplicationUser = await _applicationUserService.GetCurrentUserAsync(this.User);
            try
            {
                var invoice = await _shoppingBasketService.GetInvoiceAsync(invoiceId, ApplicationUser.Id.ToString());
                Invoice = invoice;
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError("from CheckoutModel.OnGetAsync", ex);
                throw;
            }
        }
    }
}