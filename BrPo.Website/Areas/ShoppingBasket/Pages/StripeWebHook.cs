using BrPo.Website.Services.Email;
using BrPo.Website.Services.ShoppingBasket.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using System.IO;
using System.Threading.Tasks;

namespace BrPo.Website.Areas.ShoppingBasket.Pages
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeWebHook : ControllerBase
    {
        private readonly ILogger<StripeWebHook> _logger;
        private readonly NLog.Logger _paymentLog;
        private readonly IConfiguration _configuration;
        private readonly string _secret;
        private readonly IShoppingBasketService _shoppingBasketService;

        public StripeWebHook(ILoggerFactory loggerFactory,
            ILogger<StripeWebHook> logger,
            IConfiguration configuration,
            IShoppingBasketService shoppingBasketService)
        {
            _logger = logger;
            _paymentLog = NLog.LogManager.GetLogger("payments");
            _configuration = configuration;
            _secret = _configuration["Stripe:Secret"];
            _shoppingBasketService = shoppingBasketService;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                  json,
                  Request.Headers["Stripe-Signature"],
                  _secret
                );
                string invoiceId = "missing", userId = "missing", stripeObjectId = "missing";
                if (stripeEvent.Data.Object is Stripe.PaymentIntent)
                {
                    var paymentIntent = stripeEvent.Data.Object as Stripe.PaymentIntent;
                    paymentIntent.Metadata.TryGetValue("invoiceid", out invoiceId);
                    paymentIntent.Metadata.TryGetValue("userid", out userId);
                    userId = userId.Replace("\"", string.Empty);
                    stripeObjectId = paymentIntent.Id;
                }
                if (stripeEvent.Data.Object is Stripe.Charge)
                {
                    var charge = stripeEvent.Data.Object as Stripe.Charge;
                    charge.Metadata.TryGetValue("invoiceid", out invoiceId);
                    charge.Metadata.TryGetValue("userid", out userId);
                    stripeObjectId = charge.Id;
                }
                var message = $"invoiceId; {invoiceId},   event: {stripeEvent.Type},   eventId: {stripeEvent.Id},    userId: {userId},  stripeObjectId: {stripeObjectId}";
                _paymentLog.Info(message);

                if (stripeEvent.Type == Events.PaymentIntentCreated)
                {
                }
                else if (stripeEvent.Type == Events.ChargeSucceeded)
                {
                }
                else if (stripeEvent.Type == Events.ChargeFailed)
                {
                    var charge = stripeEvent.Data.Object as Stripe.Charge;
                    var fraudMessage = string.Empty;
                    if (charge.FraudDetails?.StripeReport != null)
                        fraudMessage = charge.FraudDetails?.StripeReport?.ToString() ?? string.Empty;
                    if (charge.FraudDetails?.UserReport != null)
                        fraudMessage += "\r\n" + charge.FraudDetails?.UserReport?.ToString() ?? string.Empty;
                    message = $"invoiceId; {invoiceId},   event: {stripeEvent.Type},    failureCode: {charge.FailureCode},  failureMessage: {charge.FailureMessage},   fraudMessage: {fraudMessage}";
                    _paymentLog.Info(message);
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    await this.FulfillOrder(invoiceId.ToInt(), userId, stripeObjectId);
                }
                else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                }
                else if (stripeEvent.Type == Events.PaymentIntentCanceled)
                {
                }
                else if (stripeEvent.Type == Events.PaymentIntentProcessing)
                {
                }
                else if (stripeEvent.Type == Events.PaymentIntentCreated)
                {
                }
                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError($"from StripeWebHook: {ex.ToMessageString()}", ex);
                return BadRequest();
            }
        }

        private async Task FulfillOrder(int invoiceId, string userId, string paymentIntentId)
        {
            await _shoppingBasketService.MarkInvoiceAsPaidAsync(invoiceId, userId, paymentIntentId);
        }
    }
}