using Microsoft.Extensions.Logging;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Calculators;

/// <summary>
/// Complex calculator with advanced business rules and multi-phase calculations
/// </summary>
public class ComplexCalculator : BaseCalculator
{
    private readonly PrimaryCalculator _primaryCalculator;
    
    public override string CalculatorName => "Complex Calculator";
    public override string Version => "3.0.0";
    
    public ComplexCalculator(
        ILogger<ComplexCalculator> logger,
        PrimaryCalculator primaryCalculator) : base(logger)
    {
        _primaryCalculator = primaryCalculator ?? throw new ArgumentNullException(nameof(primaryCalculator));
    }
    
    public override async Task<CalculationResult> CalculateAsync(
        Product product,
        int quantity,
        Dictionary<string, object> parameters,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        Logger.LogInformation("Starting complex calculation for product {ProductId}", product.Id);
        
        var result = CreateResult(product.Id, quantity, product.Price);
        AddMetadata(result, "CalculatorType", CalculatorName);
        AddMetadata(result, "CalculatorVersion", Version);
        
        // Multi-phase complex calculation
        await ExecutePhase1_BaselineCalculation(result, product, quantity, parameters, cancellationToken);
        await ExecutePhase2_AdvancedModifiers(result, product, parameters, cancellationToken);
        await ExecutePhase3_DynamicPricing(result, product, quantity, parameters, cancellationToken);
        await ExecutePhase4_RiskAssessment(result, product, quantity, cancellationToken);
        await ExecutePhase5_MarketAnalysis(result, parameters, cancellationToken);
        await ExecutePhase6_CompetitivePositioning(result, product, parameters, cancellationToken);
        await ExecutePhase7_ValueOptimization(result, product, quantity, cancellationToken);
        await ExecutePhase8_FinalAdjustments(result, parameters, cancellationToken);
        
        result.Subtotal = Round(result.BaseAmount);
        
        var discountPercentage = GetParameter(parameters, "discountPercentage", 0m);
        if (discountPercentage > 0) ApplyDiscount(result, discountPercentage);
        
        var taxPercentage = GetParameter(parameters, "taxPercentage", 0m);
        if (taxPercentage > 0) ApplyTax(result, taxPercentage);
        
        FinalizeResult(result, startTime);
        return result;
    }
    
    private async Task ExecutePhase1_BaselineCalculation(
        CalculationResult result, Product product, int quantity, 
        Dictionary<string, object> parameters, CancellationToken ct)
    {
        await Task.Delay(5, ct);
        
        var baselineMultiplier = GetParameter(parameters, "baselineMultiplier", 1.0m);
        var adjustmentFactor = GetParameter(parameters, "adjustmentFactor", 0m);
        
        result.BaseAmount = product.Price * quantity * baselineMultiplier;
        if (adjustmentFactor != 0)
        {
            result.BaseAmount *= (1 + adjustmentFactor);
        }
        
        AddStep(result, "Phase 1: Baseline calculation", result.BaseAmount, 
            $"Price * Quantity * {baselineMultiplier} * (1 + {adjustmentFactor})");
        AddMetadata(result, "BaselineMultiplier", baselineMultiplier);
    }
    
    private async Task ExecutePhase2_AdvancedModifiers(
        CalculationResult result, Product product, 
        Dictionary<string, object> parameters, CancellationToken ct)
    {
        await Task.Delay(5, ct);
        
        var modifierSets = new Dictionary<string, decimal>();
        
        // Collect all modifier parameters
        foreach (var param in parameters.Where(p => p.Key.StartsWith("modifier_")))
        {
            if (decimal.TryParse(param.Value.ToString(), out var value))
            {
                modifierSets[param.Key] = value;
            }
        }
        
        decimal totalModifier = 1.0m;
        foreach (var modifier in modifierSets)
        {
            totalModifier *= (1 + modifier.Value);
        }
        
        if (totalModifier != 1.0m)
        {
            var oldAmount = result.BaseAmount;
            result.BaseAmount *= totalModifier;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "AdvancedModifiers",
                Description = $"Applied {modifierSets.Count} advanced modifiers",
                Amount = result.BaseAmount - oldAmount,
                IsAdditive = true,
                Reason = $"Total modifier: {totalModifier:F4}"
            });
            
            AddStep(result, "Phase 2: Advanced modifiers", result.BaseAmount);
        }
        
        AddMetadata(result, "ModifierCount", modifierSets.Count);
    }
    
    private async Task ExecutePhase3_DynamicPricing(
        CalculationResult result, Product product, int quantity,
        Dictionary<string, object> parameters, CancellationToken ct)
    {
        await Task.Delay(5, ct);
        
        var demandScore = CalculateDemandScore(product, quantity);
        var supplyScore = CalculateSupplyScore(product, quantity);
        var elasticity = GetParameter(parameters, "priceElasticity", 1.0m);
        
        var dynamicFactor = 1.0m + ((demandScore - supplyScore) * elasticity * 0.1m);
        dynamicFactor = Math.Max(0.5m, Math.Min(2.0m, dynamicFactor));
        
        if (Math.Abs(dynamicFactor - 1.0m) > 0.01m)
        {
            var oldAmount = result.BaseAmount;
            result.BaseAmount *= dynamicFactor;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "DynamicPricing",
                Description = $"Dynamic pricing adjustment (demand: {demandScore}, supply: {supplyScore})",
                Amount = result.BaseAmount - oldAmount,
                IsAdditive = dynamicFactor > 1.0m,
                Reason = $"Dynamic factor: {dynamicFactor:F4}, Elasticity: {elasticity}"
            });
            
            AddStep(result, "Phase 3: Dynamic pricing", result.BaseAmount);
        }
        
        AddMetadata(result, "DemandScore", demandScore);
        AddMetadata(result, "SupplyScore", supplyScore);
        AddMetadata(result, "DynamicFactor", dynamicFactor);
    }
    
    private decimal CalculateDemandScore(Product product, int quantity)
    {
        var score = 0m;
        
        if (product.IsActive) score += 0.3m;
        if (quantity > 100) score += 0.2m;
        if (product.Type == ProductType.Premium) score += 0.3m;
        if (product.StockQuantity < product.ReorderLevel) score += 0.4m;
        
        return Math.Min(1.0m, score);
    }
    
    private decimal CalculateSupplyScore(Product product, int quantity)
    {
        var score = 0m;
        
        if (product.StockQuantity > quantity * 5) score += 0.4m;
        if (product.Type == ProductType.Standard) score += 0.3m;
        if (product.StockQuantity > product.ReorderLevel * 2) score += 0.3m;
        
        return Math.Min(1.0m, score);
    }
    
    private async Task ExecutePhase4_RiskAssessment(
        CalculationResult result, Product product, int quantity, CancellationToken ct)
    {
        await Task.Delay(5, ct);
        
        var riskFactors = new Dictionary<string, decimal>();
        
        // Market risk
        if (product.Type == ProductType.Custom || product.Type == ProductType.Industrial)
            riskFactors["market"] = 0.05m;
        
        // Inventory risk
        if (product.StockQuantity < quantity)
            riskFactors["inventory"] = 0.08m;
        
        // Volume risk
        if (quantity > 1000)
            riskFactors["volume"] = 0.04m;
        
        // Price volatility risk
        if (product.Price > 5000)
            riskFactors["volatility"] = 0.03m;
        
        var totalRisk = riskFactors.Values.Sum();
        
        if (totalRisk > 0)
        {
            var riskPremium = result.BaseAmount * totalRisk;
            result.BaseAmount += riskPremium;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "RiskPremium",
                Description = $"Risk assessment premium ({riskFactors.Count} factors)",
                Amount = riskPremium,
                IsAdditive = true,
                Reason = $"Total risk: {totalRisk:P2}"
            });
            
            AddStep(result, "Phase 4: Risk assessment", result.BaseAmount);
        }
        
        AddMetadata(result, "RiskFactors", riskFactors.Count);
        AddMetadata(result, "TotalRisk", totalRisk);
    }
    
    private async Task ExecutePhase5_MarketAnalysis(
        CalculationResult result, Dictionary<string, object> parameters, CancellationToken ct)
    {
        await Task.Delay(5, ct);
        
        var marketTrend = GetParameter(parameters, "marketTrend", "stable");
        var competitorCount = GetParameter(parameters, "competitorCount", 5);
        var marketShare = GetParameter(parameters, "marketShare", 0.15m);
        
        var trendMultiplier = marketTrend?.ToLowerInvariant() switch
        {
            "declining" => 0.92m,
            "stable" => 1.0m,
            "growing" => 1.08m,
            "booming" => 1.15m,
            _ => 1.0m
        };
        
        var competitionFactor = 1.0m - (competitorCount * 0.01m);
        competitionFactor = Math.Max(0.85m, competitionFactor);
        
        var shareFactor = 1.0m + (marketShare * 0.2m);
        
        var marketFactor = trendMultiplier * competitionFactor * shareFactor;
        
        if (Math.Abs(marketFactor - 1.0m) > 0.01m)
        {
            var oldAmount = result.BaseAmount;
            result.BaseAmount *= marketFactor;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "MarketAnalysis",
                Description = $"Market analysis adjustment (trend: {marketTrend})",
                Amount = result.BaseAmount - oldAmount,
                IsAdditive = marketFactor > 1.0m,
                Reason = $"Market factor: {marketFactor:F4}"
            });
            
            AddStep(result, "Phase 5: Market analysis", result.BaseAmount);
        }
        
        AddMetadata(result, "MarketTrend", marketTrend ?? "stable");
        AddMetadata(result, "MarketFactor", marketFactor);
    }
    
    private async Task ExecutePhase6_CompetitivePositioning(
        CalculationResult result, Product product, 
        Dictionary<string, object> parameters, CancellationToken ct)
    {
        await Task.Delay(5, ct);
        
        var targetPosition = GetParameter(parameters, "targetPosition", "mid-market");
        var brandStrength = GetParameter(parameters, "brandStrength", 0.5m);
        var differentiationScore = GetParameter(parameters, "differentiationScore", 0.5m);
        
        var positionMultiplier = targetPosition?.ToLowerInvariant() switch
        {
            "budget" => 0.85m,
            "mid-market" => 1.0m,
            "premium" => 1.25m,
            "luxury" => 1.60m,
            _ => 1.0m
        };
        
        var brandAdjustment = 1.0m + (brandStrength * 0.3m);
        var differentiationAdjustment = 1.0m + (differentiationScore * 0.2m);
        
        var competitiveFactor = positionMultiplier * brandAdjustment * differentiationAdjustment;
        
        if (Math.Abs(competitiveFactor - 1.0m) > 0.01m)
        {
            var oldAmount = result.BaseAmount;
            result.BaseAmount *= competitiveFactor;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "Competitive",
                Description = $"Competitive positioning (position: {targetPosition})",
                Amount = result.BaseAmount - oldAmount,
                IsAdditive = competitiveFactor > 1.0m,
                Reason = $"Competitive factor: {competitiveFactor:F4}"
            });
            
            AddStep(result, "Phase 6: Competitive positioning", result.BaseAmount);
        }
        
        AddMetadata(result, "TargetPosition", targetPosition ?? "mid-market");
        AddMetadata(result, "CompetitiveFactor", competitiveFactor);
    }
    
    private async Task ExecutePhase7_ValueOptimization(
        CalculationResult result, Product product, int quantity, CancellationToken ct)
    {
        await Task.Delay(5, ct);
        
        var valueScore = CalculateValueScore(product, quantity);
        var optimizationPotential = CalculateOptimizationPotential(product, quantity);
        
        var optimizationFactor = 1.0m - (valueScore * optimizationPotential * 0.1m);
        optimizationFactor = Math.Max(0.9m, Math.Min(1.0m, optimizationFactor));
        
        if (optimizationFactor < 1.0m)
        {
            var reduction = result.BaseAmount * (1.0m - optimizationFactor);
            result.BaseAmount -= reduction;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "ValueOptimization",
                Description = "Value optimization adjustment",
                Amount = reduction,
                IsAdditive = false,
                Reason = $"Optimization factor: {optimizationFactor:F4}, Value score: {valueScore:F2}"
            });
            
            AddStep(result, "Phase 7: Value optimization", result.BaseAmount);
        }
        
        AddMetadata(result, "ValueScore", valueScore);
        AddMetadata(result, "OptimizationPotential", optimizationPotential);
    }
    
    private decimal CalculateValueScore(Product product, int quantity)
    {
        var score = 0m;
        
        if (product.Type == ProductType.Standard) score += 0.3m;
        if (quantity >= 100) score += 0.2m;
        if (product.IsActive && product.StockQuantity >= quantity) score += 0.3m;
        if (product.Price < 1000) score += 0.2m;
        
        return Math.Min(1.0m, score);
    }
    
    private decimal CalculateOptimizationPotential(Product product, int quantity)
    {
        var potential = 0m;
        
        if (product.StockQuantity > quantity * 3) potential += 0.3m;
        if (product.Type == ProductType.Standard) potential += 0.3m;
        if (quantity >= 50) potential += 0.4m;
        
        return Math.Min(1.0m, potential);
    }
    
    private async Task ExecutePhase8_FinalAdjustments(
        CalculationResult result, Dictionary<string, object> parameters, CancellationToken ct)
    {
        await Task.Delay(5, ct);
        
        var customAdjustment = GetParameter(parameters, "customAdjustment", 0m);
        var roundingPreference = GetParameter(parameters, "roundingPreference", "standard");
        
        if (customAdjustment != 0)
        {
            result.BaseAmount += customAdjustment;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "Custom",
                Description = "Custom adjustment",
                Amount = customAdjustment,
                IsAdditive = customAdjustment > 0,
                Reason = "Manual adjustment specified"
            });
            
            AddStep(result, "Phase 8: Custom adjustment", result.BaseAmount);
        }
        
        result.BaseAmount = ApplyRoundingPreference(result.BaseAmount, roundingPreference);
        AddStep(result, $"Final rounding ({roundingPreference})", result.BaseAmount);
        
        AddMetadata(result, "RoundingPreference", roundingPreference ?? "standard");
    }
    
    private decimal ApplyRoundingPreference(decimal amount, string? preference)
    {
        return preference?.ToLowerInvariant() switch
        {
            "none" => amount,
            "nearest5" => Math.Round(amount / 5) * 5,
            "nearest10" => Math.Round(amount / 10) * 10,
            "nearest100" => Math.Round(amount / 100) * 100,
            "up" => Math.Ceiling(amount),
            "down" => Math.Floor(amount),
            _ => Round(amount)
        };
    }
}
