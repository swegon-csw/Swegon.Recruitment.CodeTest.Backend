using System.ComponentModel.DataAnnotations;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

/// <summary>
/// Request for performing a calculation
/// </summary>
public class CalculationRequest
{
    /// <summary>
    /// Product ID to calculate for
    /// </summary>
    [Required]
    public Guid ProductId { get; set; }
    
    /// <summary>
    /// Quantity for calculation
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Input parameters for calculation
    /// </summary>
    [Required]
    public Dictionary<string, object> Parameters { get; set; } = new();
    
    /// <summary>
    /// Whether to apply discount
    /// </summary>
    public bool ApplyDiscount { get; set; }
    
    /// <summary>
    /// Discount percentage if applicable
    /// </summary>
    [Range(0, 100)]
    public decimal? DiscountPercentage { get; set; }
    
    /// <summary>
    /// Include detailed breakdown in response
    /// </summary>
    public bool IncludeBreakdown { get; set; }
    
    /// <summary>
    /// Calculation options
    /// </summary>
    public CalculationOptions? Options { get; set; }
}

/// <summary>
/// Additional options for calculation
/// </summary>
public class CalculationOptions
{
    /// <summary>
    /// Use cached results if available
    /// </summary>
    public bool UseCache { get; set; } = true;
    
    /// <summary>
    /// Precision level for calculations
    /// </summary>
    public int Precision { get; set; } = 2;
    
    /// <summary>
    /// Currency code for result
    /// </summary>
    public string Currency { get; set; } = "USD";
}
