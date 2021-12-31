using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrPo.Website.Services.ShoppingBasket.Models
{
    public class BasketItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Guid UserId { get; set;  }
        [Required]
        public int PrintOrderId { get; set; }
        [ForeignKey(nameof(PrintOrderId))]
        public PrintOrder PrintOrder { get; set; }
    }
}
