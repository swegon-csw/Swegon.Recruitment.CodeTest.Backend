using Microsoft.Extensions.Logging;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Calculators;

public class SecondaryCalculator : BaseCalculator
{
    public override string CalculatorName => "Secondary Calculator";
    
    public SecondaryCalculator(ILogger<SecondaryCalculator> logger) : base(logger) {}
    
    public override async Task<CalculationResult> CalculateAsync(
        Product product, int quantity, Dictionary<string, object> parameters,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var result = CreateResult(product.Id, quantity, product.Price);
        
        var factor = GetParameter(parameters, "secondaryFactor", 1.2m);
        result.BaseAmount *= factor;
        result.Subtotal = Round(result.BaseAmount);
        
        AddStep(result, "Secondary calculation applied", result.Subtotal);
        
        FinalizeResult(result, startTime);
        return await Task.FromResult(result);
    }
}
