using System.ComponentModel.DataAnnotations;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

/// <summary>
/// Product domain model representing a product in the system
/// </summary>
public class Product : EntityModel
{
    /// <summary>
    /// Product name
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed product description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Type of product (Standard, Premium, Custom, Industrial)
    /// </summary>
    [Required]
    public ProductType Type { get; set; }
    
    /// <summary>
    /// Stock Keeping Unit - unique product identifier
    /// </summary>
    [StringLength(50)]
    public string? SKU { get; set; }
    
    /// <summary>
    /// Product price in the default currency
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    /// <summary>
    /// Whether the product is currently active and available
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Technical specifications and attributes as key-value pairs
    /// </summary>
    public Dictionary<string, string> Specifications { get; set; } = new();
    
    /// <summary>
    /// Weight in kilograms
    /// </summary>
    public decimal? Weight { get; set; }
    
    /// <summary>
    /// Length in centimeters
    /// </summary>
    public decimal? Length { get; set; }
    
    /// <summary>
    /// Width in centimeters
    /// </summary>
    public decimal? Width { get; set; }
    
    /// <summary>
    /// Height in centimeters
    /// </summary>
    public decimal? Height { get; set; }
    
    /// <summary>
    /// Manufacturer name
    /// </summary>
    [StringLength(100)]
    public string? Manufacturer { get; set; }
    
    /// <summary>
    /// Current stock quantity
    /// </summary>
    public int StockQuantity { get; set; }
    
    /// <summary>
    /// Reorder level for inventory management
    /// </summary>
    public int ReorderLevel { get; set; }
    
    /// <summary>
    /// Product category
    /// </summary>
    [StringLength(50)]
    public string? Category { get; set; }
    
    /// <summary>
    /// Tags for search and categorization
    /// </summary>
    public List<string> Tags { get; set; } = new();
    
    /// <summary>
    /// Calculates the volume in cubic centimeters
    /// </summary>
    public decimal? CalculateVolume()
    {
        if (Length.HasValue && Width.HasValue && Height.HasValue)
        {
            return Length.Value * Width.Value * Height.Value;
        }
        return null;
    }
    
    /// <summary>
    /// Checks if the product needs to be reordered
    /// </summary>
    public bool NeedsReorder() => StockQuantity <= ReorderLevel;
}
