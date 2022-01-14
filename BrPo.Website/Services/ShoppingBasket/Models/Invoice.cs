using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrPo.Website.Services.ShoppingBasket.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        [Required]
        public bool IsComplete { get; set; }

        [Required]
        public Decimal ItemsSubTotal { get; set; }

        [Required]
        public Decimal VoucherDiscount { get; set; }

        [Required]
        public Decimal Delivery { get; set; }

        [Required]
        public Decimal Total { get; set; }

        [MaxLength(500)]
        public string Note { get; set; }

        [MaxLength(50)]
        public string StrikePaymentId { get; set; }

        public virtual ICollection<PrintInvoiceItem> Items { get; set; }

        public Invoice(Guid userId, decimal itemsSubTotal, decimal voucherDiscount, decimal delivery)
        {
            UserId = userId;
            CreatedDate = DateTime.UtcNow;
            //PaymentDate = DateTime.MinValue;
            IsComplete = false;
            ItemsSubTotal = itemsSubTotal;
            VoucherDiscount = voucherDiscount;
            Delivery = delivery;
            Total = ItemsSubTotal - VoucherDiscount + Delivery;
            Items = new List<PrintInvoiceItem>();
        }
    }
}