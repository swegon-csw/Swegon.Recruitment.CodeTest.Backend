using System.ComponentModel.DataAnnotations;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

public class CalculationRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    public Dictionary<string, object> Parameters { get; set; } = new();

    public bool ApplyDiscount { get; set; }

    [Range(0, 100)]
    public decimal? DiscountPercentage { get; set; }

    public bool IncludeBreakdown { get; set; }

    public CalculationOptions? Options { get; set; }
}

public class CalculationOptions
{
    public bool UseCache { get; set; } = true;

    public int Precision { get; set; } = 2;

    public string Currency { get; set; } = "USD";
}
