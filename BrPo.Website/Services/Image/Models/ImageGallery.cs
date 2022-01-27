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

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime DateUpdated { get; set; }

        public int CoverImageId { get; set; }
        public int ContentCount { get; set; }
        public string GalleryRootName { get; set; }

        public virtual IEnumerable<ImageGalleryContent> Content { get; set; }
    }
}