using Newtonsoft.Json;

namespace BrPo.Website.Areas.ShoppingBasket.Models;

public class PaymentIntentCreateRequest
{
    [JsonProperty("amount")]
    public int Amount { get; set; }

    [JsonProperty("invoiceid")]
    public int InvoiceId { get; set; }

    [JsonProperty("userid")]
    public string UserId { get; set; }
}