using BrPo.Website.Areas.ShoppingBasket.Models;
using BrPo.Website.Services.Email;
using BrPo.Website.Services.ShoppingBasket.Models;
using BrPo.Website.Services.ShoppingBasket.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrPo.Website.Areas.ShoppingBasket.Pages
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentIntentController : ControllerBase
    {
        private readonly ILogger<PaymentIntentController> _logger;
        private readonly IShoppingBasketService _shoppingBasketService;

        public PaymentIntentController(
            ILogger<PaymentIntentController> logger,
            IShoppingBasketService shoppingBasketService)
        {
            _logger = logger;
            _shoppingBasketService = shoppingBasketService;
        }

        [HttpPost]
        public async Task<IActionResult> Index(PaymentIntentCreateRequest request)
        {
            if (request.Amount <= 0)
                return null;
            var paymentIntentService = new PaymentIntentService();
            var metadata = new Dictionary<string, string>();
            metadata.Add("invoiceid", JsonConvert.SerializeObject(request.InvoiceId));
            metadata.Add("userid", JsonConvert.SerializeObject(request.UserId));
            if (await AmountMatchesInvoice(request.Amount, metadata["invoiceid"].ToInt(), metadata["userid"].Replace("\"", "")))
            {
                var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
                {
                    Amount = request.Amount,
                    Currency = "gbp",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                    Metadata = metadata
                });
                return new JsonResult(new { clientSecret = paymentIntent.ClientSecret });
            }
            else
            {
                return new BadRequestObjectResult(new { error = "Incorrect amount in request" });
            }
        }

        private async Task<bool> AmountMatchesInvoice(int amount, int invoiceId, string userId)
        {
            var invoiceTotal = await _shoppingBasketService.GetInvoiceTotalAsync(invoiceId, userId);
            return invoiceTotal == (Decimal)amount / 100;
        }
    }
}