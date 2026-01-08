using System.ComponentModel.DataAnnotations;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

/// <summary>
/// Calculation model representing a calculation operation
/// </summary>
public class Calculation : EntityModel
{
    /// <summary>
    /// Reference to the product this calculation is for
    /// </summary>
    [Required]
    public Guid ProductId { get; set; }
    
    /// <summary>
    /// Current status of the calculation
    /// </summary>
    public CalculationStatus Status { get; set; } = CalculationStatus.Pending;
    
    /// <summary>
    /// Final calculated total
    /// </summary>
    public decimal Total { get; set; }
    
    /// <summary>
    /// Subtotal before discounts and taxes
    /// </summary>
    public decimal Subtotal { get; set; }
    
    /// <summary>
    /// Total discount amount applied
    /// </summary>
    public decimal DiscountAmount { get; set; }
    
    /// <summary>
    /// Total tax amount
    /// </summary>
    public decimal TaxAmount { get; set; }
    
    /// <summary>
    /// Currency code (ISO 4217)
    /// </summary>
    [StringLength(3)]
    public string Currency { get; set; } = "USD";
    
    /// <summary>
    /// Additional metadata for the calculation
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
    
    /// <summary>
    /// When the calculation was performed
    /// </summary>
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Quantity used in calculation
    /// </summary>
    public int Quantity { get; set; } = 1;
    
    /// <summary>
    /// Input parameters used for calculation
    /// </summary>
    public Dictionary<string, object> InputParameters { get; set; } = new();
    
    /// <summary>
    /// Detailed breakdown of calculation steps
    /// </summary>
    public List<CalculationStep> Breakdown { get; set; } = new();
    
    /// <summary>
    /// Error message if calculation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Discount percentage applied
    /// </summary>
    public decimal? DiscountPercentage { get; set; }
    
    /// <summary>
    /// Tax percentage applied
    /// </summary>
    public decimal? TaxPercentage { get; set; }
    
    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }
    
    /// <summary>
    /// Calculation version for tracking algorithm changes
    /// </summary>
    public string CalculationVersion { get; set; } = "1.0";
}

/// <summary>
/// Represents a single step in the calculation breakdown
/// </summary>
public class CalculationStep
{
    /// <summary>
    /// Step number in the calculation sequence
    /// </summary>
    public int StepNumber { get; set; }
    
    /// <summary>
    /// Description of the calculation step
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Value at this step
    /// </summary>
    public decimal Value { get; set; }
    
    /// <summary>
    /// Formula or method used
    /// </summary>
    public string? Formula { get; set; }
    
    /// <summary>
    /// Additional notes for this step
    /// </summary>
    public string? Notes { get; set; }
}
