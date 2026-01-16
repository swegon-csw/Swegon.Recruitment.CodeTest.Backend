using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

public class CalculationResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public CalculationStatus Status { get; set; }
    public decimal Total { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public CalculationBreakdown? Breakdown { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public DateTime CalculatedAt { get; set; }
    public long DurationMs { get; set; }
}

public class CalculationBreakdown
{
    public decimal BaseCost { get; set; }
    public Dictionary<string, decimal> AdditionalCosts { get; set; } = new();
    public List<DiscountInfo> Discounts { get; set; } = new();
    public Dictionary<string, decimal> Taxes { get; set; } = new();
}

public class DiscountInfo
{
    public string Name { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public decimal Amount { get; set; }
}
