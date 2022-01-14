using BrPo.Website.Areas.ShoppingBasket.Models;
using BrPo.Website.Services.Email;
using BrPo.Website.Services.Image.Services;
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrPo.Website.Areas.ShoppingBasket.Pages
{
    public class BasketModel : PageModel
    {
        private readonly ILogger<BasketModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IImageService _imageService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IShoppingBasketService _shoppingBasketService;

        public List<BasketItem> Items { get; set; }
        public decimal BasketTotalValue { get; set; }

        public BasketModel(
            ILogger<BasketModel> logger,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IHttpContextAccessor httpContextAccessor,
            IImageService imageService,
            UserManager<IdentityUser> userManager,
            IShoppingBasketService shoppingBasketService)
        {
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _imageService = imageService;
            _userManager = userManager;
            _shoppingBasketService = shoppingBasketService;
        }

        public void OnGet()
        {
            var userId = GetUserOrSessionId();
            Items = _shoppingBasketService.GetBasketItems(userId);
            BasketTotalValue = Math.Round(Items.Sum(i => i.PrintOrderItem.Value), 2);
        }

        private string GetUserOrSessionId()
        {
            var principle = this.User;
            if (principle.Identity.IsAuthenticated)
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

        public PartialViewResult OnGetBasketItemsPartial()
        {
            var userId = GetUserOrSessionId();
            var model = _shoppingBasketService.GetBasketItems(userId);
            return Partial("BasketItemsPartial", model);
        }

        public IActionResult OnGetBasketTotal()
        {
            var userId = GetUserOrSessionId();
            var total = _shoppingBasketService.GetBasketTotal(userId);
            return new JsonResult(total);
        }

        public async Task<IActionResult> OnPostQuantityChange(string basketItemId, string newQuantity)
        {
            var userId = GetUserOrSessionId();
            await _shoppingBasketService.ChangeQuantityAsync(userId, basketItemId.ToInt(), newQuantity.ToInt());
            return new OkResult();
        }

        public async Task<IActionResult> OnPostDeleteItem(string basketItemId)
        {
            var userId = GetUserOrSessionId();
            await _shoppingBasketService.DeleteItemAsync(userId, basketItemId.ToInt());
            return new OkResult();
        }

        public async Task<IActionResult> OnPostGoToPayment(CreateInvoiceModel createInvoiceModel)
        {
            try
            {
                var userId = GetUserOrSessionId();
                //var total = _shoppingBasketService.GetBasketTotal(userId);
                //var domain = "https://" + Request.Host.Value;
                //var options = new SessionCreateOptions
                //{
                //    LineItems = new List<SessionLineItemOptions>
                //    {
                //      new SessionLineItemOptions
                //      {
                //        PriceData = new SessionLineItemPriceDataOptions
                //        {
                //            Currency = "gbp",
                //            ProductData = new SessionLineItemPriceDataProductDataOptions
                //            {
                //                Name = "Printing Services",
                //                Description = "For customer invoice XXX"
                //            },
                //            UnitAmount = (int)(total * 100)
                //        },
                //        Quantity = 1
                //      }
                //    },
                //    Mode = "payment",
                //    SuccessUrl = domain + "/ShoppingBasket/Success?session_id={CHECKOUT_SESSION_ID}",
                //    CancelUrl = domain + "/ShoppingBasket/Cancel?session_id={CHECKOUT_SESSION_ID}",
                //    ClientReferenceId = userId,
                //};
                //var service = new SessionService();
                //Session session = service.Create(options);
                //return new StatusCodeResult(303);
                //return new OkObjectResult(session.Url);

                try
                {
                    var invoice = await _shoppingBasketService.CreateInvoiceAsync(userId, createInvoiceModel);
                    return new OkObjectResult($"Checkout?invoiceId={invoice.Id}");
                }
                catch (Exception ex)
                {
                    return new BadRequestObjectResult(ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("from ShoppingBasket.OnPostGoToPayment", ex);
                throw;
            }
        }

        public async Task<IActionResult> OnPostUserId()
        {
            await Task.Delay(5);
            var userId = GetUserOrSessionId();
            return new OkObjectResult(userId);
        }
    }
}