using Microsoft.Extensions.Logging;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Calculators;

/// <summary>
/// Utility calculator for common calculation operations
/// </summary>
public class UtilityCalculator : BaseCalculator
{
    public override string CalculatorName => "Utility Calculator";
    
    public UtilityCalculator(ILogger<UtilityCalculator> logger) : base(logger) {}
    
    public override async Task<CalculationResult> CalculateAsync(
        Product product, int quantity, Dictionary<string, object> parameters,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var result = CreateResult(product.Id, quantity, product.Price);
        
        var utilityFactor = GetParameter(parameters, "utilityFactor", 1.1m);
        result.BaseAmount *= utilityFactor;
        result.Subtotal = Round(result.BaseAmount);
        
        AddStep(result, "Utility calculation applied", result.Subtotal);
        FinalizeResult(result, startTime);
        
        return await Task.FromResult(result);
    }
}
