using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

/// <summary>
/// Response for calculation operations
/// </summary>
public class CalculationResponse
{
    /// <summary>
    /// Calculation ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Product ID
    /// </summary>
    public Guid ProductId { get; set; }
    
    /// <summary>
    /// Calculation status
    /// </summary>
    public CalculationStatus Status { get; set; }
    
    /// <summary>
    /// Calculated total
    /// </summary>
    public decimal Total { get; set; }
    
    /// <summary>
    /// Subtotal before discounts
    /// </summary>
    public decimal Subtotal { get; set; }
    
    /// <summary>
    /// Discount amount applied
    /// </summary>
    public decimal DiscountAmount { get; set; }
    
    /// <summary>
    /// Tax amount
    /// </summary>
    public decimal TaxAmount { get; set; }
    
    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = "USD";
    
    /// <summary>
    /// Detailed calculation breakdown
    /// </summary>
    public CalculationBreakdown? Breakdown { get; set; }
    
    /// <summary>
    /// Calculation result metadata
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
    
    /// <summary>
    /// Timestamp when calculation was performed
    /// </summary>
    public DateTime CalculatedAt { get; set; }
    
    /// <summary>
    /// Calculation duration in milliseconds
    /// </summary>
    public long DurationMs { get; set; }
}

/// <summary>
/// Detailed breakdown of a calculation
/// </summary>
public class CalculationBreakdown
{
    /// <summary>
    /// Base cost
    /// </summary>
    public decimal BaseCost { get; set; }
    
    /// <summary>
    /// Additional costs
    /// </summary>
    public Dictionary<string, decimal> AdditionalCosts { get; set; } = new();
    
    /// <summary>
    /// Applied discounts
    /// </summary>
    public List<DiscountInfo> Discounts { get; set; } = new();
    
    /// <summary>
    /// Tax breakdown
    /// </summary>
    public Dictionary<string, decimal> Taxes { get; set; } = new();
}

/// <summary>
/// Information about an applied discount
/// </summary>
public class DiscountInfo
{
    /// <summary>
    /// Discount name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Discount percentage
    /// </summary>
    public decimal Percentage { get; set; }
    
    /// <summary>
    /// Discount amount
    /// </summary>
    public decimal Amount { get; set; }
}
