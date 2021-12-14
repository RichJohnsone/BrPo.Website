using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrPo.Website.Services.Image.Models
{
    [Table("ImageFiles")]
    public class ImageFileModel
    {
        [Key]
        public int Id { get; set; }   
        [Required]
        [MaxLength(50)]
        public string UserId { get; set; }
        [Required]
        [MaxLength(255)]
        public string OriginalFileName { get; set; }
        [Required]
        [MaxLength(255)]
        public string Location { get; set; }
        [Required]
        public int Height { get; set; }
        [Required]
        public int Width { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
    }
}
