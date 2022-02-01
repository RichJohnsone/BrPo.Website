using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BrPo.Website.Services.Paper.Models.Enums;

namespace BrPo.Website.Services.Image.Models
{
    [Table("ImageGalleryItems")]
    public class ImageGalleryItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int ImageFileId { get; set; }

        [Required]
        public int MinHeight { get; set; }

        [Required]
        public int MinWidth { get; set; }

        [Required]
        public int MaxHeight { get; set; }

        [Required]
        public int MaxWidth { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public int MinPrice { get; set; }

        [Required]
        public int MaxPrice { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [EnumDataType(typeof(PaperSurface))]
        public PaperSurface? PaperSurface { get; set; }

        [EnumDataType(typeof(PaperTexture))]
        public PaperTexture? PaperTexture { get; set; }

        [ForeignKey(nameof(ImageFileId))]
        public virtual ImageFileModel ImageFile { get; set; }

        public virtual List<ImageTag> Tags { get; set; }

        [NotMapped]
        public virtual List<ImageGallery> Galleries { get; set; }
    }
}