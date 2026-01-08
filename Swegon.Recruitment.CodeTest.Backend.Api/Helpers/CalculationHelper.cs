namespace Swegon.Recruitment.CodeTest.Backend.Api.Helpers;

/// <summary>
/// Helper class for calculation utilities and formulas
/// </summary>
public static class CalculationHelper
{
    /// <summary>
    /// Calculates discount amount from a percentage
    /// </summary>
    public static decimal CalculateDiscountAmount(decimal amount, decimal discountPercentage)
    {
        if (discountPercentage < 0 || discountPercentage > 100)
            throw new ArgumentOutOfRangeException(nameof(discountPercentage), "Discount percentage must be between 0 and 100");
        
        return amount * (discountPercentage / 100m);
    }
    
    /// <summary>
    /// Calculates tax amount from a percentage
    /// </summary>
    public static decimal CalculateTaxAmount(decimal amount, decimal taxPercentage)
    {
        if (taxPercentage < 0)
            throw new ArgumentOutOfRangeException(nameof(taxPercentage), "Tax percentage cannot be negative");
        
        return amount * (taxPercentage / 100m);
    }
    
    /// <summary>
    /// Calculates the final total with discount and tax
    /// </summary>
    public static decimal CalculateTotal(decimal subtotal, decimal discountAmount, decimal taxAmount)
    {
        var afterDiscount = subtotal - discountAmount;
        return afterDiscount + taxAmount;
    }
    
    /// <summary>
    /// Calculates percentage change between two values
    /// </summary>
    public static decimal CalculatePercentageChange(decimal oldValue, decimal newValue)
    {
        if (oldValue == 0)
            return 0;
        
        return ((newValue - oldValue) / oldValue) * 100m;
    }
    
    /// <summary>
    /// Calculates compound interest
    /// </summary>
    public static decimal CalculateCompoundInterest(decimal principal, decimal rate, int periods)
    {
        if (periods <= 0)
            throw new ArgumentOutOfRangeException(nameof(periods), "Periods must be greater than zero");
        
        return principal * (decimal)Math.Pow((double)(1 + rate / 100m), periods);
    }
    
    /// <summary>
    /// Calculates weighted average
    /// </summary>
    public static decimal CalculateWeightedAverage(Dictionary<decimal, decimal> valuesAndWeights)
    {
        if (valuesAndWeights == null || valuesAndWeights.Count == 0)
            throw new ArgumentException("Values and weights cannot be empty", nameof(valuesAndWeights));
        
        var totalWeight = valuesAndWeights.Sum(kvp => kvp.Value);
        if (totalWeight == 0)
            throw new ArgumentException("Total weight cannot be zero", nameof(valuesAndWeights));
        
        var weightedSum = valuesAndWeights.Sum(kvp => kvp.Key * kvp.Value);
        return weightedSum / totalWeight;
    }
    
    /// <summary>
    /// Rounds to the nearest specified precision
    /// </summary>
    public static decimal RoundToNearest(decimal value, decimal precision)
    {
        if (precision <= 0)
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must be greater than zero");
        
        return Math.Round(value / precision) * precision;
    }
    
    /// <summary>
    /// Rounds currency to 2 decimal places
    /// </summary>
    public static decimal RoundCurrency(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
    
    /// <summary>
    /// Calculates markup amount
    /// </summary>
    public static decimal CalculateMarkup(decimal cost, decimal markupPercentage)
    {
        if (markupPercentage < 0)
            throw new ArgumentOutOfRangeException(nameof(markupPercentage), "Markup percentage cannot be negative");
        
        return cost * (markupPercentage / 100m);
    }
    
    /// <summary>
    /// Calculates margin percentage
    /// </summary>
    public static decimal CalculateMarginPercentage(decimal sellingPrice, decimal cost)
    {
        if (sellingPrice == 0)
            return 0;
        
        return ((sellingPrice - cost) / sellingPrice) * 100m;
    }
    
    /// <summary>
    /// Calculates break-even point
    /// </summary>
    public static decimal CalculateBreakEvenPoint(decimal fixedCosts, decimal pricePerUnit, decimal variableCostPerUnit)
    {
        var contributionMargin = pricePerUnit - variableCostPerUnit;
        if (contributionMargin <= 0)
            throw new ArgumentException("Contribution margin must be positive");
        
        return fixedCosts / contributionMargin;
    }
    
    /// <summary>
    /// Calculates ROI (Return on Investment)
    /// </summary>
    public static decimal CalculateROI(decimal gain, decimal cost)
    {
        if (cost == 0)
            throw new ArgumentException("Cost cannot be zero", nameof(cost));
        
        return ((gain - cost) / cost) * 100m;
    }
    
    /// <summary>
    /// Applies a progressive discount based on quantity
    /// </summary>
    public static decimal ApplyProgressiveDiscount(int quantity, decimal basePrice)
    {
        decimal discountPercentage = quantity switch
        {
            >= 1000 => 25m,
            >= 500 => 20m,
            >= 100 => 15m,
            >= 50 => 10m,
            >= 10 => 5m,
            _ => 0m
        };
        
        return discountPercentage;
    }
    
    /// <summary>
    /// Calculates depreciation using straight-line method
    /// </summary>
    public static decimal CalculateStraightLineDepreciation(decimal cost, decimal salvageValue, int usefulLife)
    {
        if (usefulLife <= 0)
            throw new ArgumentOutOfRangeException(nameof(usefulLife), "Useful life must be greater than zero");
        
        return (cost - salvageValue) / usefulLife;
    }
    
    /// <summary>
    /// Calculates NPV (Net Present Value)
    /// </summary>
    public static decimal CalculateNPV(decimal[] cashFlows, decimal discountRate)
    {
        if (cashFlows == null || cashFlows.Length == 0)
            throw new ArgumentException("Cash flows cannot be empty", nameof(cashFlows));
        
        decimal npv = 0;
        for (int i = 0; i < cashFlows.Length; i++)
        {
            npv += cashFlows[i] / (decimal)Math.Pow((double)(1 + discountRate), i);
        }
        
        return npv;
    }
    
    /// <summary>
    /// Interpolates a value between two points
    /// </summary>
    public static decimal Interpolate(decimal x, decimal x0, decimal y0, decimal x1, decimal y1)
    {
        if (x0 == x1)
            throw new ArgumentException("x0 and x1 cannot be equal");
        
        return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
    }
}
