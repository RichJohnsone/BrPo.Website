using BrPo.Website.Services.ApplicationUser.Models;
using BrPo.Website.Services.ApplicationUser.Services;
using BrPo.Website.Services.ShoppingBasket.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BrPo.Website.Areas.ShoppingBasket.Pages;

public class SuccessModel : PageModel
{
    private readonly ILogger<SuccessModel> _logger;
    private readonly IShoppingBasketService _shoppingBasketService;
    private readonly IApplicationUserService _applicationUserService;

    public ApplicationUser ApplicationUser { get; set; }

    public SuccessModel(
        ILogger<SuccessModel> logger,
        IShoppingBasketService shoppingBasketService,
        IApplicationUserService applicationUserService)
    {
        _logger = logger;
        _shoppingBasketService = shoppingBasketService;
        _applicationUserService = applicationUserService;
    }

    public async Task<IActionResult> OnGet([FromQuery] string session_id)
    {
        ApplicationUser = await _applicationUserService.GetCurrentUserAsync(this.User);
        return Page();
    }
}