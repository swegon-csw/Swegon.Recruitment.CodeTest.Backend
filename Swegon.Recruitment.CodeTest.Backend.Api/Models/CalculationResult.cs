using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

/// <summary>
/// Detailed calculation result with comprehensive breakdown
/// </summary>
public class CalculationResult
{
    /// <summary>
    /// Calculation identifier
    /// </summary>
    public Guid CalculationId { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Product identifier
    /// </summary>
    public Guid ProductId { get; set; }
    
    /// <summary>
    /// Calculation status
    /// </summary>
    public CalculationStatus Status { get; set; }
    
    /// <summary>
    /// Base amount before any adjustments
    /// </summary>
    public decimal BaseAmount { get; set; }
    
    /// <summary>
    /// Subtotal after quantity multiplication
    /// </summary>
    public decimal Subtotal { get; set; }
    
    /// <summary>
    /// Total discount applied
    /// </summary>
    public decimal DiscountAmount { get; set; }
    
    /// <summary>
    /// Tax amount
    /// </summary>
    public decimal TaxAmount { get; set; }
    
    /// <summary>
    /// Final total amount
    /// </summary>
    public decimal Total { get; set; }
    
    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = "USD";
    
    /// <summary>
    /// Quantity used in calculation
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Unit price
    /// </summary>
    public decimal UnitPrice { get; set; }
    
    /// <summary>
    /// Discount percentage applied
    /// </summary>
    public decimal DiscountPercentage { get; set; }
    
    /// <summary>
    /// Tax percentage applied
    /// </summary>
    public decimal TaxPercentage { get; set; }
    
    /// <summary>
    /// Detailed calculation steps
    /// </summary>
    public List<CalculationStep> Steps { get; set; } = new();
    
    /// <summary>
    /// Applied adjustments
    /// </summary>
    public List<CalculationAdjustment> Adjustments { get; set; } = new();
    
    /// <summary>
    /// Calculation formulas used
    /// </summary>
    public Dictionary<string, string> Formulas { get; set; } = new();
    
    /// <summary>
    /// Intermediate calculation values
    /// </summary>
    public Dictionary<string, decimal> IntermediateValues { get; set; } = new();
    
    /// <summary>
    /// Calculation timestamp
    /// </summary>
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Processing duration in milliseconds
    /// </summary>
    public long ProcessingDurationMs { get; set; }
    
    /// <summary>
    /// Warnings generated during calculation
    /// </summary>
    public List<string> Warnings { get; set; } = new();
    
    /// <summary>
    /// Errors encountered during calculation
    /// </summary>
    public List<string> Errors { get; set; } = new();
    
    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Represents an adjustment applied during calculation
/// </summary>
public class CalculationAdjustment
{
    /// <summary>
    /// Adjustment type
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Adjustment description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Adjustment amount
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Whether the adjustment is additive or subtractive
    /// </summary>
    public bool IsAdditive { get; set; }
    
    /// <summary>
    /// Reason for the adjustment
    /// </summary>
    public string? Reason { get; set; }
}
