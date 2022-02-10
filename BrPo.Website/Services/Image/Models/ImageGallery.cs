using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrPo.Website.Services.Image.Models
{
    [Table("ImageGalleries")]
    public class ImageGallery
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Updated")]
        [Required]
        public DateTime DateUpdated { get; set; }

        public int CoverImageId { get; set; }

        public int ContentCount { get; set; }

        [Required]
        [MaxLength(30)]
        [Display(Name = "Gallery root name")]
        public string GalleryRootName { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public virtual IEnumerable<ImageGalleryContent> Content { get; set; }
    }
}