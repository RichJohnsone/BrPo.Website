using System;
using System.ComponentModel.DataAnnotations;

namespace BrPo.Website.Services.ShoppingBasket.Models
{
    public interface IPrintItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int FileId { get; set; }

        [Required]
        public int PaperId { get; set; }

        [Required]
        public int Height { get; set; }

        [Required]
        public int Width { get; set; }

        [Required]
        public int Border { get; set; }

        [Required]
        [MaxLength(20)]
        public string Quality { get; set; }

        [Required]
        public bool IsDraft { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public Decimal Value { get; set; }
    }
}