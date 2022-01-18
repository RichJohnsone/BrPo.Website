using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrPo.Website.Services.ApplicationUser.Models
{
    [Table("GuestUsers")]
    public class GuestUserModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public GuestUserModel(Guid id)
        {
            Id = id;
            DateCreated = DateTime.UtcNow;
        }
    }
}