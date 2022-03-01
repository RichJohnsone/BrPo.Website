using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrPo.Website.Services.Image.Models;

[Table("ImageGalleryContent")]
public class ImageGalleryContent
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ImageGalleryId { get; set; }

    [Required]
    public int ImageGalleryItemId { get; set; }

    [Required]
    public DateTime DateCreated { get; set; }

    [ForeignKey(nameof(ImageGalleryItemId))]
    public virtual ImageGalleryItem ImageGalleryItem { get; set; }
}