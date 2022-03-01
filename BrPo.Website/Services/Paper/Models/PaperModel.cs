using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BrPo.Website.Services.Paper.Models.Enums;

namespace BrPo.Website.Services.Paper.Models;

[BindProperties]
[Table("Papers")]
public class PaperModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    [Required]
    [MaxLength(255)]
    public string Description { get; set; }

    [Required]
    public int GSMWeight { get; set; }

    [Required]
    public bool RollPaper { get; set; }

    [Required]
    public DateTime DateCreated { get; set; }

    [MaxLength(100)]
    public string ColourProfilePath { get; set; }

    public int RollWidth { get; set; }
    public int CutSheetHeight { get; set; }
    public int CutSheetWidth { get; set; }

    [Required]
    public bool IsActive { get; set; }

    [MaxLength(50)]
    public string ProductCode { get; set; }

    public Decimal CostPerMeter { get; set; }
    public Decimal CostPerSheet { get; set; }

    [Required]
    [EnumDataType(typeof(PaperSurface))]
    public PaperSurface PaperSurface { get; set; }

    [Required]
    [EnumDataType(typeof(PaperTexture))]
    public PaperTexture PaperTexture { get; set; }
}