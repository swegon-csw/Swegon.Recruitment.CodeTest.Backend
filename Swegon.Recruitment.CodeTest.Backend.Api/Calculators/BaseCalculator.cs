using Microsoft.Extensions.Logging;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Calculators;

/// <summary>
/// Abstract base calculator with common calculation functionality
/// </summary>
public abstract class BaseCalculator
{
    protected readonly ILogger Logger;
    
    /// <summary>
    /// Initializes the base calculator
    /// </summary>
    protected BaseCalculator(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// Executes the calculation
    /// </summary>
    public abstract Task<CalculationResult> CalculateAsync(
        Product product,
        int quantity,
        Dictionary<string, object> parameters,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the calculator name
    /// </summary>
    public abstract string CalculatorName { get; }
    
    /// <summary>
    /// Gets the calculator version
    /// </summary>
    public virtual string Version => "1.0";
    
    /// <summary>
    /// Validates calculation inputs
    /// </summary>
    protected virtual ValidationResult ValidateInputs(Product product, int quantity, Dictionary<string, object> parameters)
    {
        var result = new ValidationResult { IsValid = true };
        
        if (product == null)
        {
            result.AddError("Product", "Product is required", ErrorCode.ValidationError);
        }
        
        if (quantity <= 0)
        {
            result.AddError("Quantity", "Quantity must be greater than zero", ErrorCode.ValidationError);
        }
        
        if (parameters == null)
        {
            result.AddError("Parameters", "Parameters cannot be null", ErrorCode.ValidationError);
        }
        
        return result;
    }
    
    /// <summary>
    /// Creates a base calculation result
    /// </summary>
    protected CalculationResult CreateResult(Guid productId, int quantity, decimal unitPrice)
    {
        return new CalculationResult
        {
            CalculationId = Guid.NewGuid(),
            ProductId = productId,
            Status = CalculationStatus.InProgress,
            Quantity = quantity,
            UnitPrice = unitPrice,
            BaseAmount = unitPrice * quantity,
            CalculatedAt = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Adds a calculation step to the result
    /// </summary>
    protected void AddStep(CalculationResult result, string description, decimal value, string? formula = null)
    {
        result.Steps.Add(new CalculationStep
        {
            StepNumber = result.Steps.Count + 1,
            Description = description,
            Value = value,
            Formula = formula,
            Notes = $"Step executed at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"
        });
    }
    
    /// <summary>
    /// Applies discount to a calculation result
    /// </summary>
    protected void ApplyDiscount(CalculationResult result, decimal discountPercentage)
    {
        if (discountPercentage <= 0)
            return;
        
        result.DiscountPercentage = discountPercentage;
        result.DiscountAmount = result.Subtotal * (discountPercentage / 100m);
        
        AddStep(result, $"Apply {discountPercentage}% discount", result.DiscountAmount, 
            $"Subtotal * ({discountPercentage} / 100)");
    }
    
    /// <summary>
    /// Applies tax to a calculation result
    /// </summary>
    protected void ApplyTax(CalculationResult result, decimal taxPercentage)
    {
        if (taxPercentage <= 0)
            return;
        
        result.TaxPercentage = taxPercentage;
        var taxableAmount = result.Subtotal - result.DiscountAmount;
        result.TaxAmount = taxableAmount * (taxPercentage / 100m);
        
        AddStep(result, $"Apply {taxPercentage}% tax", result.TaxAmount,
            $"(Subtotal - Discount) * ({taxPercentage} / 100)");
    }
    
    /// <summary>
    /// Finalizes the calculation result
    /// </summary>
    protected void FinalizeResult(CalculationResult result, DateTime startTime)
    {
        result.Total = result.Subtotal - result.DiscountAmount + result.TaxAmount;
        result.ProcessingDurationMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;
        result.Status = CalculationStatus.Completed;
        
        AddStep(result, "Final total calculated", result.Total,
            "Subtotal - Discount + Tax");
        
        Logger.LogInformation(
            "Calculation {CalculationId} completed for product {ProductId} in {Duration}ms. Total: {Total}",
            result.CalculationId, result.ProductId, result.ProcessingDurationMs, result.Total);
    }
    
    /// <summary>
    /// Gets a parameter value or default
    /// </summary>
    protected T? GetParameter<T>(Dictionary<string, object> parameters, string key, T? defaultValue = default)
    {
        if (!parameters.TryGetValue(key, out var value))
            return defaultValue;
        
        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            Logger.LogWarning("Failed to convert parameter {Key} to type {Type}", key, typeof(T).Name);
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Rounds a value to 2 decimal places
    /// </summary>
    protected decimal Round(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
    
    /// <summary>
    /// Adds metadata to the result
    /// </summary>
    protected void AddMetadata(CalculationResult result, string key, object value)
    {
        result.Metadata[key] = value;
    }
}
