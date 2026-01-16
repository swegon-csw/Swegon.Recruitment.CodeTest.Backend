using System.ComponentModel.DataAnnotations;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

public class Product : EntityModel
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public ProductType Type { get; set; }

    [StringLength(50)]
    public string? SKU { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    public Dictionary<string, string> Specifications { get; set; } = new();

    public decimal? Weight { get; set; }

    public decimal? Length { get; set; }

    public decimal? Width { get; set; }

    public decimal? Height { get; set; }

    [StringLength(100)]
    public string? Manufacturer { get; set; }

    public int StockQuantity { get; set; }

    public int ReorderLevel { get; set; }

    [StringLength(50)]
    public string? Category { get; set; }

    public List<string> Tags { get; set; } = new();

    public decimal? CalculateVolume()
    {
        if (Length.HasValue && Width.HasValue && Height.HasValue)
        {
            return Length.Value * Width.Value * Height.Value;
        }
        return null;
    }

    public bool NeedsReorder() => StockQuantity <= ReorderLevel;
}
