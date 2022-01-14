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
        public Guid UserId { get; set; }

        [Required]
        public int PrintOrderItemId { get; set; }

        [ForeignKey(nameof(PrintOrderItemId))]
        public PrintOrderItem PrintOrderItem { get; set; }
    }
}