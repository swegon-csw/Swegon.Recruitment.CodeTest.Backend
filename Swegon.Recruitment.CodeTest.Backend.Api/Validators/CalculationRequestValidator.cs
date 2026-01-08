using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Validators;

/// <summary>
/// Validator for calculation requests
/// </summary>
public class CalculationRequestValidator
{
    /// <summary>
    /// Validates a calculation request
    /// </summary>
    public ValidationResult Validate(CalculationRequest request)
    {
        var result = new ValidationResult { IsValid = true, ValidatorName = nameof(CalculationRequestValidator) };
        
        ValidateProductId(request, result);
        ValidateQuantity(request, result);
        ValidateParameters(request, result);
        ValidateDiscount(request, result);
        ValidateOptions(request, result);
        
        return result;
    }
    
    /// <summary>
    /// Validates product ID
    /// </summary>
    private void ValidateProductId(CalculationRequest request, ValidationResult result)
    {
        if (request.ProductId == Guid.Empty)
        {
            result.AddError(nameof(request.ProductId), "Product ID is required", ErrorCode.ValidationError);
        }
    }
    
    /// <summary>
    /// Validates quantity
    /// </summary>
    private void ValidateQuantity(CalculationRequest request, ValidationResult result)
    {
        if (request.Quantity <= 0)
        {
            result.AddError(nameof(request.Quantity), "Quantity must be greater than zero", ErrorCode.ValidationError);
        }
        else if (request.Quantity > 100000)
        {
            result.AddError(nameof(request.Quantity), "Quantity exceeds maximum allowed value", ErrorCode.ValidationError);
        }
        else if (request.Quantity > 10000)
        {
            result.AddWarning(nameof(request.Quantity), "Large quantity may result in longer processing time");
        }
    }
    
    /// <summary>
    /// Validates calculation parameters
    /// </summary>
    private void ValidateParameters(CalculationRequest request, ValidationResult result)
    {
        if (request.Parameters == null)
        {
            result.AddError(nameof(request.Parameters), "Parameters cannot be null", ErrorCode.ValidationError);
            return;
        }
        
        if (request.Parameters.Count == 0)
        {
            result.AddWarning(nameof(request.Parameters), "No calculation parameters provided");
        }
        
        if (request.Parameters.Count > 100)
        {
            result.AddWarning(nameof(request.Parameters), "Large number of parameters may affect performance");
        }
        
        // Validate specific known parameters
        ValidateNumericParameter(request.Parameters, "multiplier", result, 0, 10);
        ValidateNumericParameter(request.Parameters, "factor", result, 0, 100);
        ValidateNumericParameter(request.Parameters, "rate", result, 0, 1);
        
        // Check for required parameters
        var requiredParams = new[] { "baseValue", "calculationType" };
        foreach (var param in requiredParams)
        {
            if (!request.Parameters.ContainsKey(param))
            {
                result.AddWarning(nameof(request.Parameters), $"Recommended parameter '{param}' is missing");
            }
        }
    }
    
    /// <summary>
    /// Validates a numeric parameter
    /// </summary>
    private void ValidateNumericParameter(Dictionary<string, object> parameters, string key, ValidationResult result, decimal min, decimal max)
    {
        if (!parameters.ContainsKey(key))
            return;
        
        var value = parameters[key];
        if (value == null)
        {
            result.AddWarning(key, $"Parameter '{key}' is null");
            return;
        }
        
        decimal numericValue;
        try
        {
            numericValue = Convert.ToDecimal(value);
        }
        catch
        {
            result.AddError(key, $"Parameter '{key}' must be a numeric value", ErrorCode.ValidationError);
            return;
        }
        
        if (numericValue < min || numericValue > max)
        {
            result.AddError(key, $"Parameter '{key}' must be between {min} and {max}", ErrorCode.ValidationError);
        }
    }
    
    /// <summary>
    /// Validates discount settings
    /// </summary>
    private void ValidateDiscount(CalculationRequest request, ValidationResult result)
    {
        if (request.ApplyDiscount)
        {
            if (!request.DiscountPercentage.HasValue)
            {
                result.AddError(nameof(request.DiscountPercentage), "Discount percentage is required when applying discount", ErrorCode.ValidationError);
            }
            else if (request.DiscountPercentage.Value < 0)
            {
                result.AddError(nameof(request.DiscountPercentage), "Discount percentage cannot be negative", ErrorCode.ValidationError);
            }
            else if (request.DiscountPercentage.Value > 100)
            {
                result.AddError(nameof(request.DiscountPercentage), "Discount percentage cannot exceed 100", ErrorCode.ValidationError);
            }
            else if (request.DiscountPercentage.Value > 50)
            {
                result.AddWarning(nameof(request.DiscountPercentage), "Discount percentage is unusually high");
            }
            else if (request.DiscountPercentage.Value == 0)
            {
                result.AddWarning(nameof(request.DiscountPercentage), "Apply discount is enabled but percentage is zero");
            }
        }
        else if (request.DiscountPercentage.HasValue && request.DiscountPercentage.Value > 0)
        {
            result.AddWarning(nameof(request.DiscountPercentage), "Discount percentage is set but apply discount is disabled");
        }
    }
    
    /// <summary>
    /// Validates calculation options
    /// </summary>
    private void ValidateOptions(CalculationRequest request, ValidationResult result)
    {
        if (request.Options == null)
        {
            return;
        }
        
        // Validate tax settings
        if (request.Options.TaxPercentage < 0)
        {
            result.AddError("Options.TaxPercentage", "Tax percentage cannot be negative", ErrorCode.ValidationError);
        }
        else if (request.Options.TaxPercentage > 50)
        {
            result.AddError("Options.TaxPercentage", "Tax percentage exceeds reasonable limit", ErrorCode.ValidationError);
        }
        
        // Validate currency
        if (!string.IsNullOrEmpty(request.Options.Currency) && request.Options.Currency.Length != 3)
        {
            result.AddError("Options.Currency", "Currency code must be 3 characters (ISO 4217)", ErrorCode.ValidationError);
        }
        
        // Validate rounding
        if (request.Options.RoundingDigits < 0 || request.Options.RoundingDigits > 10)
        {
            result.AddError("Options.RoundingDigits", "Rounding digits must be between 0 and 10", ErrorCode.ValidationError);
        }
    }
    
    /// <summary>
    /// Validates a batch calculation request
    /// </summary>
    public ValidationResult ValidateBatch(BatchRequest<CalculationRequest> batchRequest)
    {
        var result = new ValidationResult { IsValid = true, ValidatorName = nameof(CalculationRequestValidator) };
        
        if (batchRequest == null)
        {
            result.AddError("BatchRequest", "Batch request cannot be null", ErrorCode.ValidationError);
            return result;
        }
        
        if (batchRequest.Items == null || batchRequest.Items.Count == 0)
        {
            result.AddError("Items", "Batch request must contain at least one item", ErrorCode.ValidationError);
            return result;
        }
        
        if (batchRequest.Items.Count > 100)
        {
            result.AddError("Items", "Batch request cannot contain more than 100 items", ErrorCode.ValidationError);
        }
        else if (batchRequest.Items.Count > 50)
        {
            result.AddWarning("Items", "Large batch size may result in longer processing time");
        }
        
        // Validate each item in the batch
        int itemIndex = 0;
        foreach (var item in batchRequest.Items)
        {
            var itemValidation = Validate(item);
            if (!itemValidation.IsValid)
            {
                foreach (var error in itemValidation.Errors)
                {
                    result.AddError($"Items[{itemIndex}].{error.Field}", error.Message, error.ErrorCode);
                }
            }
            itemIndex++;
        }
        
        return result;
    }
    
    /// <summary>
    /// Validates parameters for a specific calculation type
    /// </summary>
    public ValidationResult ValidateForCalculationType(CalculationRequest request, string calculationType)
    {
        var result = Validate(request);
        
        var requiredParameters = GetRequiredParametersForType(calculationType);
        
        foreach (var param in requiredParameters)
        {
            if (!request.Parameters.ContainsKey(param))
            {
                result.AddError(nameof(request.Parameters), 
                    $"Required parameter '{param}' for calculation type '{calculationType}' is missing", 
                    ErrorCode.ValidationError);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Gets required parameters for a calculation type
    /// </summary>
    private List<string> GetRequiredParametersForType(string calculationType)
    {
        return calculationType?.ToLowerInvariant() switch
        {
            "standard" => new List<string> { "baseValue" },
            "premium" => new List<string> { "baseValue", "premiumFactor" },
            "custom" => new List<string> { "baseValue", "customMultiplier", "adjustmentFactor" },
            "industrial" => new List<string> { "baseValue", "industrialCoefficient", "complexityLevel" },
            _ => new List<string>()
        };
    }
}
