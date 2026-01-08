using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

/// <summary>
/// Response for product operations
/// </summary>
public class ProductResponse
{
    /// <summary>
    /// Product ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Product name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Product description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Product type
    /// </summary>
    public ProductType Type { get; set; }
    
    /// <summary>
    /// Product SKU
    /// </summary>
    public string? Sku { get; set; }
    
    /// <summary>
    /// Product price
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Whether the product is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Product specifications
    /// </summary>
    public Dictionary<string, string>? Specifications { get; set; }
    
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Created by user ID
    /// </summary>
    public string? CreatedBy { get; set; }
}
