using Microsoft.Extensions.Logging;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Helpers;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Calculators;

/// <summary>
/// Discount calculator for handling various discount scenarios
/// </summary>
public class DiscountCalculator : BaseCalculator
{
    public override string CalculatorName => "Discount Calculator";
    
    public DiscountCalculator(ILogger<DiscountCalculator> logger) : base(logger) {}
    
    public override async Task<CalculationResult> CalculateAsync(
        Product product, int quantity, Dictionary<string, object> parameters,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var result = CreateResult(product.Id, quantity, product.Price);
        
        result.Subtotal = result.BaseAmount;
        
        var discountPercentage = CalculationHelper.ApplyProgressiveDiscount(quantity, product.Price);
        if (discountPercentage > 0)
        {
            ApplyDiscount(result, discountPercentage);
        }
        
        FinalizeResult(result, startTime);
        return await Task.FromResult(result);
    }
}
