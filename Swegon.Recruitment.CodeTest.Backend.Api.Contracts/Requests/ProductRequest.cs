using System.ComponentModel.DataAnnotations;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

/// <summary>
/// Request for creating or updating a product
/// </summary>
public class ProductRequest
{
    /// <summary>
    /// Product name
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Product description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Product type
    /// </summary>
    [Required]
    public ProductType Type { get; set; }
    
    /// <summary>
    /// Product SKU
    /// </summary>
    [StringLength(50)]
    public string? Sku { get; set; }
    
    /// <summary>
    /// Product price
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    /// <summary>
    /// Whether the product is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Product specifications
    /// </summary>
    public Dictionary<string, string>? Specifications { get; set; }
}
