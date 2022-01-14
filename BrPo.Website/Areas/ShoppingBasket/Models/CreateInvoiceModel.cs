using Newtonsoft.Json;

namespace BrPo.Website.Areas.ShoppingBasket.Models
{
    public class CreateInvoiceModel
    {
        [JsonProperty("vouchercode")]
        public string VoucherCode { get; set; }

        [JsonProperty("voucherdiscount")]
        public decimal VoucherDiscount { get; set; }

        [JsonProperty("delivery")]
        public decimal Delivery { get; set; }
    }
}