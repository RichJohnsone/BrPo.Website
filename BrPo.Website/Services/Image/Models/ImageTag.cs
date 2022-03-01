using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrPo.Website.Services.Image.Models;

[Table("ImageTags")]
[Index(nameof(Tag))]
public class ImageTag
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public int ImageGalleryItemId { get; set; }

    [Required]
    [MaxLength(50)]
    [MinLength(3)]
    public string Tag { get; set; }

    [Required]
    public DateTime DateCreated { get; set; }
}