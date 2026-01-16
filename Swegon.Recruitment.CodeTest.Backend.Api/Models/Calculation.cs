using System.ComponentModel.DataAnnotations;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

public class Calculation : EntityModel
{
    [Required]
    public Guid ProductId { get; set; }

    public CalculationStatus Status { get; set; } = CalculationStatus.Pending;

    public decimal Total { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    [StringLength(3)]
    public string Currency { get; set; } = "USD";

    public Dictionary<string, object> Metadata { get; set; } = new();

    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

    public int Quantity { get; set; } = 1;

    public Dictionary<string, object> InputParameters { get; set; } = new();

    public List<CalculationStep> Breakdown { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public decimal? TaxPercentage { get; set; }

    public long ProcessingTimeMs { get; set; }

    public string CalculationVersion { get; set; } = "1.0";
}

public class CalculationStep
{
    public int StepNumber { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Value { get; set; }

    public string? Formula { get; set; }

    public string? Notes { get; set; }
}
