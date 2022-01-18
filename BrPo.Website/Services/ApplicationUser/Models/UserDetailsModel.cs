using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrPo.Website.Services.ApplicationUser.Models
{
    [Table("UserDetails")]
    public class UserDetailsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Address1 { get; set; }

        [MaxLength(100)]
        public string Address2 { get; set; }

        [MaxLength(100)]
        public string Address3 { get; set; }

        [Required]
        [MaxLength(50)]
        public string PostalTown { get; set; }

        [Required]
        [MaxLength(20)]
        public string Postcode { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }
    }
}