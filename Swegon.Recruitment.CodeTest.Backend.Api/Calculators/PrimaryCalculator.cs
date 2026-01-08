using Microsoft.Extensions.Logging;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;
using Swegon.Recruitment.CodeTest.Backend.Api.Helpers;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Calculators;

/// <summary>
/// Primary calculator implementing main calculation logic with complex formulas
/// </summary>
public class PrimaryCalculator : BaseCalculator
{
    private const decimal BasePremiumMultiplier = 1.35m;
    private const decimal CustomComplexityFactor = 1.75m;
    private const decimal IndustrialScaleFactor = 2.25m;
    private const decimal MinimumCalculationValue = 0.01m;
    private const decimal MaximumCalculationValue = 10000000m;
    
    public override string CalculatorName => "Primary Calculator";
    public override string Version => "2.1.0";
    
    public PrimaryCalculator(ILogger<PrimaryCalculator> logger) : base(logger)
    {
    }
    
    /// <summary>
    /// Executes the primary calculation with comprehensive business logic
    /// </summary>
    public override async Task<CalculationResult> CalculateAsync(
        Product product,
        int quantity,
        Dictionary<string, object> parameters,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        Logger.LogInformation("Starting primary calculation for product {ProductId}, quantity {Quantity}", 
            product.Id, quantity);
        
        // Validate inputs
        var validation = ValidateInputs(product, quantity, parameters);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException($"Validation failed: {string.Join(", ", validation.Errors.Select(e => e.Message))}");
        }
        
        // Initialize result
        var result = CreateResult(product.Id, quantity, product.Price);
        AddMetadata(result, "CalculatorType", CalculatorName);
        AddMetadata(result, "CalculatorVersion", Version);
        AddMetadata(result, "ProductType", product.Type.ToString());
        
        // Phase 1: Calculate base values
        await CalculateBaseValuesAsync(result, product, quantity, parameters, cancellationToken);
        
        // Phase 2: Apply product type specific calculations
        await ApplyProductTypeCalculationsAsync(result, product, parameters, cancellationToken);
        
        // Phase 3: Apply volume-based adjustments
        ApplyVolumeAdjustments(result, quantity, product);
        
        // Phase 4: Apply complexity factors
        await ApplyComplexityFactorsAsync(result, product, parameters, cancellationToken);
        
        // Phase 5: Calculate specifications impact
        CalculateSpecificationsImpact(result, product);
        
        // Phase 6: Apply dimensional calculations
        ApplyDimensionalCalculations(result, product);
        
        // Phase 7: Calculate weight-based adjustments
        CalculateWeightBasedAdjustments(result, product, quantity);
        
        // Phase 8: Apply market factors
        ApplyMarketFactors(result, product, parameters);
        
        // Phase 9: Calculate seasonal adjustments
        ApplySeasonalAdjustments(result, parameters);
        
        // Phase 10: Apply region-specific calculations
        ApplyRegionSpecificCalculations(result, parameters);
        
        // Phase 11: Calculate risk premiums
        CalculateRiskPremiums(result, product, quantity);
        
        // Phase 12: Apply optimization factors
        ApplyOptimizationFactors(result, product, quantity);
        
        // Final calculations
        result.Subtotal = Round(result.BaseAmount);
        AddStep(result, "Calculated subtotal", result.Subtotal);
        
        // Apply discount if specified
        var discountPercentage = GetParameter(parameters, "discountPercentage", 0m);
        if (discountPercentage > 0)
        {
            ApplyDiscount(result, discountPercentage);
        }
        
        // Apply tax if specified
        var taxPercentage = GetParameter(parameters, "taxPercentage", 0m);
        if (taxPercentage > 0)
        {
            ApplyTax(result, taxPercentage);
        }
        
        FinalizeResult(result, startTime);
        
        return await Task.FromResult(result);
    }
    
    /// <summary>
    /// Calculates base values for the calculation
    /// </summary>
    private async Task CalculateBaseValuesAsync(
        CalculationResult result,
        Product product,
        int quantity,
        Dictionary<string, object> parameters,
        CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken); // Simulate async work
        
        var baseValue = GetParameter(parameters, "baseValue", product.Price);
        var multiplier = GetParameter(parameters, "multiplier", 1.0m);
        
        result.BaseAmount = Round(baseValue * quantity * multiplier);
        AddStep(result, "Calculate base amount", result.BaseAmount, 
            $"{baseValue} * {quantity} * {multiplier}");
        
        // Apply base adjustments
        var baseAdjustment = GetParameter(parameters, "baseAdjustment", 0m);
        if (baseAdjustment != 0)
        {
            result.BaseAmount += baseAdjustment;
            AddStep(result, "Apply base adjustment", result.BaseAmount, 
                $"BaseAmount + {baseAdjustment}");
        }
        
        AddMetadata(result, "BaseValue", baseValue);
        AddMetadata(result, "Multiplier", multiplier);
    }
    
    /// <summary>
    /// Applies product type specific calculations
    /// </summary>
    private async Task ApplyProductTypeCalculationsAsync(
        CalculationResult result,
        Product product,
        Dictionary<string, object> parameters,
        CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
        
        decimal typeMultiplier = product.Type switch
        {
            ProductType.Standard => 1.0m,
            ProductType.Premium => BasePremiumMultiplier,
            ProductType.Custom => CustomComplexityFactor,
            ProductType.Industrial => IndustrialScaleFactor,
            _ => 1.0m
        };
        
        // Apply custom factors from parameters
        var customFactor = GetParameter(parameters, $"{product.Type}Factor", 0m);
        if (customFactor > 0)
        {
            typeMultiplier *= (1 + customFactor);
        }
        
        var adjustment = result.BaseAmount * (typeMultiplier - 1);
        if (adjustment > 0)
        {
            result.BaseAmount += adjustment;
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "ProductType",
                Description = $"{product.Type} type adjustment",
                Amount = adjustment,
                IsAdditive = true,
                Reason = $"Applied {product.Type} multiplier of {typeMultiplier}"
            });
            
            AddStep(result, $"Apply {product.Type} adjustment", result.BaseAmount,
                $"BaseAmount * {typeMultiplier}");
        }
        
        AddMetadata(result, "TypeMultiplier", typeMultiplier);
    }
    
    /// <summary>
    /// Applies volume-based adjustments based on quantity
    /// </summary>
    private void ApplyVolumeAdjustments(CalculationResult result, int quantity, Product product)
    {
        decimal volumeDiscount = CalculationHelper.ApplyProgressiveDiscount(quantity, product.Price);
        
        if (volumeDiscount > 0)
        {
            var discountAmount = result.BaseAmount * (volumeDiscount / 100m);
            result.BaseAmount -= discountAmount;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "VolumeDiscount",
                Description = $"Volume discount for {quantity} units",
                Amount = discountAmount,
                IsAdditive = false,
                Reason = $"Applied {volumeDiscount}% volume discount"
            });
            
            AddStep(result, $"Apply {volumeDiscount}% volume discount", result.BaseAmount,
                $"BaseAmount - (BaseAmount * {volumeDiscount} / 100)");
        }
        
        AddMetadata(result, "VolumeDiscount", volumeDiscount);
    }
    
    /// <summary>
    /// Applies complexity factors to the calculation
    /// </summary>
    private async Task ApplyComplexityFactorsAsync(
        CalculationResult result,
        Product product,
        Dictionary<string, object> parameters,
        CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
        
        var complexityLevel = GetParameter(parameters, "complexityLevel", 1);
        var complexityFactor = CalculateComplexityFactor(complexityLevel);
        
        if (complexityFactor != 1.0m)
        {
            var adjustment = result.BaseAmount * (complexityFactor - 1);
            result.BaseAmount += adjustment;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "Complexity",
                Description = $"Complexity level {complexityLevel} adjustment",
                Amount = adjustment,
                IsAdditive = true,
                Reason = $"Applied complexity factor {complexityFactor}"
            });
            
            AddStep(result, $"Apply complexity factor ({complexityLevel})", result.BaseAmount,
                $"BaseAmount * {complexityFactor}");
        }
        
        AddMetadata(result, "ComplexityLevel", complexityLevel);
        AddMetadata(result, "ComplexityFactor", complexityFactor);
    }
    
    /// <summary>
    /// Calculates complexity factor based on complexity level
    /// </summary>
    private decimal CalculateComplexityFactor(int complexityLevel)
    {
        return complexityLevel switch
        {
            1 => 1.0m,
            2 => 1.15m,
            3 => 1.35m,
            4 => 1.60m,
            5 => 2.00m,
            _ => 1.0m + (complexityLevel * 0.25m)
        };
    }
    
    /// <summary>
    /// Calculates the impact of product specifications
    /// </summary>
    private void CalculateSpecificationsImpact(CalculationResult result, Product product)
    {
        if (product.Specifications == null || product.Specifications.Count == 0)
            return;
        
        // Calculate specification complexity score
        var specScore = product.Specifications.Count * 0.02m;
        
        // Check for high-value specifications
        var highValueSpecs = new[] { "precision", "quality", "certification", "warranty" };
        foreach (var spec in product.Specifications)
        {
            if (highValueSpecs.Any(hvs => spec.Key.Contains(hvs, StringComparison.OrdinalIgnoreCase)))
            {
                specScore += 0.05m;
            }
        }
        
        specScore = Math.Min(specScore, 0.50m); // Cap at 50%
        
        if (specScore > 0)
        {
            var adjustment = result.BaseAmount * specScore;
            result.BaseAmount += adjustment;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "Specifications",
                Description = $"Specifications impact ({product.Specifications.Count} specs)",
                Amount = adjustment,
                IsAdditive = true,
                Reason = $"Specification complexity score: {specScore:P2}"
            });
            
            AddStep(result, "Apply specifications impact", result.BaseAmount,
                $"BaseAmount + (BaseAmount * {specScore})");
        }
        
        AddMetadata(result, "SpecificationCount", product.Specifications.Count);
        AddMetadata(result, "SpecificationScore", specScore);
    }
    
    /// <summary>
    /// Applies dimensional calculations based on product dimensions
    /// </summary>
    private void ApplyDimensionalCalculations(CalculationResult result, Product product)
    {
        if (!product.Length.HasValue || !product.Width.HasValue || !product.Height.HasValue)
            return;
        
        var volume = product.CalculateVolume();
        if (!volume.HasValue)
            return;
        
        // Calculate dimensional weight factor
        var dimensionalFactor = CalculateDimensionalFactor(volume.Value);
        
        if (dimensionalFactor != 1.0m)
        {
            var adjustment = result.BaseAmount * (dimensionalFactor - 1);
            result.BaseAmount += adjustment;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "Dimensional",
                Description = $"Dimensional adjustment (volume: {volume:N2} cm³)",
                Amount = adjustment,
                IsAdditive = adjustment > 0,
                Reason = $"Applied dimensional factor {dimensionalFactor}"
            });
            
            AddStep(result, "Apply dimensional factor", result.BaseAmount,
                $"BaseAmount * {dimensionalFactor}");
        }
        
        AddMetadata(result, "Volume", volume);
        AddMetadata(result, "DimensionalFactor", dimensionalFactor);
    }
    
    /// <summary>
    /// Calculates dimensional factor based on volume
    /// </summary>
    private decimal CalculateDimensionalFactor(decimal volume)
    {
        return volume switch
        {
            < 1000 => 0.95m,
            < 10000 => 1.0m,
            < 100000 => 1.10m,
            < 1000000 => 1.25m,
            _ => 1.50m
        };
    }
    
    /// <summary>
    /// Calculates weight-based adjustments
    /// </summary>
    private void CalculateWeightBasedAdjustments(CalculationResult result, Product product, int quantity)
    {
        if (!product.Weight.HasValue)
            return;
        
        var totalWeight = product.Weight.Value * quantity;
        var weightFactor = CalculateWeightFactor(totalWeight);
        
        if (weightFactor != 1.0m)
        {
            var adjustment = result.BaseAmount * (weightFactor - 1);
            result.BaseAmount += adjustment;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "Weight",
                Description = $"Weight adjustment (total: {totalWeight:N2} kg)",
                Amount = adjustment,
                IsAdditive = adjustment > 0,
                Reason = $"Applied weight factor {weightFactor}"
            });
            
            AddStep(result, "Apply weight factor", result.BaseAmount,
                $"BaseAmount * {weightFactor}");
        }
        
        AddMetadata(result, "TotalWeight", totalWeight);
        AddMetadata(result, "WeightFactor", weightFactor);
    }
    
    /// <summary>
    /// Calculates weight factor based on total weight
    /// </summary>
    private decimal CalculateWeightFactor(decimal weight)
    {
        return weight switch
        {
            < 1 => 1.0m,
            < 10 => 1.05m,
            < 50 => 1.15m,
            < 100 => 1.30m,
            _ => 1.50m
        };
    }
    
    /// <summary>
    /// Applies market factors to the calculation
    /// </summary>
    private void ApplyMarketFactors(CalculationResult result, Product product, Dictionary<string, object> parameters)
    {
        var marketDemand = GetParameter(parameters, "marketDemand", "normal");
        var competitionLevel = GetParameter(parameters, "competitionLevel", "medium");
        
        var marketFactor = CalculateMarketFactor(marketDemand, competitionLevel);
        
        if (marketFactor != 1.0m)
        {
            var adjustment = result.BaseAmount * (marketFactor - 1);
            result.BaseAmount += adjustment;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "Market",
                Description = $"Market adjustment (demand: {marketDemand}, competition: {competitionLevel})",
                Amount = adjustment,
                IsAdditive = adjustment > 0,
                Reason = $"Applied market factor {marketFactor}"
            });
            
            AddStep(result, "Apply market factors", result.BaseAmount,
                $"BaseAmount * {marketFactor}");
        }
        
        AddMetadata(result, "MarketDemand", marketDemand);
        AddMetadata(result, "CompetitionLevel", competitionLevel);
    }
    
    /// <summary>
    /// Calculates market factor based on demand and competition
    /// </summary>
    private decimal CalculateMarketFactor(string demand, string competition)
    {
        var demandMultiplier = demand?.ToLowerInvariant() switch
        {
            "low" => 0.90m,
            "normal" => 1.0m,
            "high" => 1.15m,
            "critical" => 1.30m,
            _ => 1.0m
        };
        
        var competitionMultiplier = competition?.ToLowerInvariant() switch
        {
            "low" => 1.10m,
            "medium" => 1.0m,
            "high" => 0.95m,
            _ => 1.0m
        };
        
        return demandMultiplier * competitionMultiplier;
    }
    
    /// <summary>
    /// Applies seasonal adjustments to the calculation
    /// </summary>
    private void ApplySeasonalAdjustments(CalculationResult result, Dictionary<string, object> parameters)
    {
        var currentMonth = DateTime.UtcNow.Month;
        var seasonalFactor = GetParameter(parameters, "seasonalFactor", GetSeasonalFactor(currentMonth));
        
        if (seasonalFactor != 1.0m)
        {
            var adjustment = result.BaseAmount * (seasonalFactor - 1);
            result.BaseAmount += adjustment;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "Seasonal",
                Description = $"Seasonal adjustment for month {currentMonth}",
                Amount = adjustment,
                IsAdditive = adjustment > 0,
                Reason = $"Applied seasonal factor {seasonalFactor}"
            });
            
            AddStep(result, "Apply seasonal adjustment", result.BaseAmount,
                $"BaseAmount * {seasonalFactor}");
        }
        
        AddMetadata(result, "SeasonalFactor", seasonalFactor);
    }
    
    /// <summary>
    /// Gets seasonal factor based on month
    /// </summary>
    private decimal GetSeasonalFactor(int month)
    {
        return month switch
        {
            12 or 1 or 2 => 1.10m, // Winter premium
            3 or 4 or 5 => 1.05m,  // Spring moderate
            6 or 7 or 8 => 0.95m,  // Summer discount
            9 or 10 or 11 => 1.0m, // Fall normal
            _ => 1.0m
        };
    }
    
    /// <summary>
    /// Applies region-specific calculations
    /// </summary>
    private void ApplyRegionSpecificCalculations(CalculationResult result, Dictionary<string, object> parameters)
    {
        var region = GetParameter(parameters, "region", "default");
        var regionFactor = GetRegionFactor(region);
        
        if (regionFactor != 1.0m)
        {
            var adjustment = result.BaseAmount * (regionFactor - 1);
            result.BaseAmount += adjustment;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "Regional",
                Description = $"Regional adjustment for {region}",
                Amount = adjustment,
                IsAdditive = adjustment > 0,
                Reason = $"Applied regional factor {regionFactor}"
            });
            
            AddStep(result, "Apply regional adjustment", result.BaseAmount,
                $"BaseAmount * {regionFactor}");
        }
        
        AddMetadata(result, "Region", region);
    }
    
    /// <summary>
    /// Gets region-specific factor
    /// </summary>
    private decimal GetRegionFactor(string region)
    {
        return region?.ToLowerInvariant() switch
        {
            "north-america" => 1.05m,
            "europe" => 1.10m,
            "asia" => 0.95m,
            "south-america" => 1.0m,
            "africa" => 0.90m,
            "oceania" => 1.15m,
            _ => 1.0m
        };
    }
    
    /// <summary>
    /// Calculates risk premiums based on various factors
    /// </summary>
    private void CalculateRiskPremiums(CalculationResult result, Product product, int quantity)
    {
        var riskScore = 0m;
        
        // High-value product risk
        if (product.Price > 10000)
            riskScore += 0.05m;
        
        // Large quantity risk
        if (quantity > 1000)
            riskScore += 0.03m;
        
        // Custom/Industrial type risk
        if (product.Type == ProductType.Custom || product.Type == ProductType.Industrial)
            riskScore += 0.04m;
        
        // Low stock risk
        if (product.StockQuantity < product.ReorderLevel)
            riskScore += 0.02m;
        
        if (riskScore > 0)
        {
            var adjustment = result.BaseAmount * riskScore;
            result.BaseAmount += adjustment;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "Risk",
                Description = "Risk premium adjustment",
                Amount = adjustment,
                IsAdditive = true,
                Reason = $"Applied risk score: {riskScore:P2}"
            });
            
            AddStep(result, "Apply risk premium", result.BaseAmount,
                $"BaseAmount + (BaseAmount * {riskScore})");
        }
        
        AddMetadata(result, "RiskScore", riskScore);
    }
    
    /// <summary>
    /// Applies optimization factors to reduce costs where possible
    /// </summary>
    private void ApplyOptimizationFactors(CalculationResult result, Product product, int quantity)
    {
        var optimizationScore = 0m;
        
        // Efficiency for standard products
        if (product.Type == ProductType.Standard)
            optimizationScore += 0.03m;
        
        // Bulk processing efficiency
        if (quantity >= 100)
            optimizationScore += 0.02m;
        
        // Available stock efficiency
        if (product.StockQuantity >= quantity * 2)
            optimizationScore += 0.01m;
        
        if (optimizationScore > 0)
        {
            var reduction = result.BaseAmount * optimizationScore;
            result.BaseAmount -= reduction;
            
            result.Adjustments.Add(new CalculationAdjustment
            {
                Type = "Optimization",
                Description = "Optimization efficiency reduction",
                Amount = reduction,
                IsAdditive = false,
                Reason = $"Applied optimization score: {optimizationScore:P2}"
            });
            
            AddStep(result, "Apply optimization", result.BaseAmount,
                $"BaseAmount - (BaseAmount * {optimizationScore})");
        }
        
        AddMetadata(result, "OptimizationScore", optimizationScore);
    }
}
