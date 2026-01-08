using Microsoft.Extensions.Logging;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Helpers;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Calculators;

/// <summary>
/// Tax calculator for handling tax calculations
/// </summary>
public class TaxCalculator : BaseCalculator
{
    public override string CalculatorName => "Tax Calculator";
    
    public TaxCalculator(ILogger<TaxCalculator> logger) : base(logger) {}
    
    public override async Task<CalculationResult> CalculateAsync(
        Product product, int quantity, Dictionary<string, object> parameters,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var result = CreateResult(product.Id, quantity, product.Price);
        
        result.Subtotal = result.BaseAmount;
        
        var taxPercentage = GetParameter(parameters, "taxPercentage", 0m);
        if (taxPercentage > 0)
        {
            ApplyTax(result, taxPercentage);
        }
        
        FinalizeResult(result, startTime);
        return await Task.FromResult(result);
    }
}
