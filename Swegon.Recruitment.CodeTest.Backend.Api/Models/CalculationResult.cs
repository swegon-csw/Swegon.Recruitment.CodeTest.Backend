using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

public class CalculationResult
{
    public Guid CalculationId { get; set; } = Guid.NewGuid();

    public Guid ProductId { get; set; }

    public CalculationStatus Status { get; set; }

    public decimal BaseAmount { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal Total { get; set; }

    public string Currency { get; set; } = "USD";

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountPercentage { get; set; }

    public decimal TaxPercentage { get; set; }

    public List<CalculationStep> Steps { get; set; } = new();

    public List<CalculationAdjustment> Adjustments { get; set; } = new();

    public Dictionary<string, string> Formulas { get; set; } = new();

    public Dictionary<string, decimal> IntermediateValues { get; set; } = new();

    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

    public long ProcessingDurationMs { get; set; }

    public List<string> Warnings { get; set; } = new();

    public List<string> Errors { get; set; } = new();

    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class CalculationAdjustment
{
    public string Type { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public bool IsAdditive { get; set; }

    public string? Reason { get; set; }
}
