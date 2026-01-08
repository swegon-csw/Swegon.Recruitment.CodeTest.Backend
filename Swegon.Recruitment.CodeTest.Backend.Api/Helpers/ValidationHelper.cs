using System.Text.RegularExpressions;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Helpers;

/// <summary>
/// Helper class for validation operations
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Validates a product model
    /// </summary>
    public static ValidationResult ValidateProduct(Product product)
    {
        var result = new ValidationResult { IsValid = true };
        
        if (string.IsNullOrWhiteSpace(product.Name))
        {
            result.AddError(nameof(product.Name), "Product name is required", ErrorCode.ValidationError);
        }
        else if (product.Name.Length < 3)
        {
            result.AddError(nameof(product.Name), "Product name must be at least 3 characters", ErrorCode.ValidationError);
        }
        else if (product.Name.Length > 100)
        {
            result.AddError(nameof(product.Name), "Product name must not exceed 100 characters", ErrorCode.ValidationError);
        }
        
        if (product.Price < 0)
        {
            result.AddError(nameof(product.Price), "Product price cannot be negative", ErrorCode.ValidationError);
        }
        
        if (!string.IsNullOrEmpty(product.SKU) && !IsValidSKU(product.SKU))
        {
            result.AddError(nameof(product.SKU), "Invalid SKU format", ErrorCode.ValidationError);
        }
        
        if (product.Description?.Length > 500)
        {
            result.AddError(nameof(product.Description), "Description must not exceed 500 characters", ErrorCode.ValidationError);
        }
        
        if (product.StockQuantity < 0)
        {
            result.AddError(nameof(product.StockQuantity), "Stock quantity cannot be negative", ErrorCode.ValidationError);
        }
        
        if (product.ReorderLevel < 0)
        {
            result.AddError(nameof(product.ReorderLevel), "Reorder level cannot be negative", ErrorCode.ValidationError);
        }
        
        if (product.Weight.HasValue && product.Weight.Value < 0)
        {
            result.AddError(nameof(product.Weight), "Weight cannot be negative", ErrorCode.ValidationError);
        }
        
        ValidateDimensions(product, result);
        
        return result;
    }
    
    /// <summary>
    /// Validates product dimensions
    /// </summary>
    private static void ValidateDimensions(Product product, ValidationResult result)
    {
        if (product.Length.HasValue && product.Length.Value < 0)
        {
            result.AddError(nameof(product.Length), "Length cannot be negative", ErrorCode.ValidationError);
        }
        
        if (product.Width.HasValue && product.Width.Value < 0)
        {
            result.AddError(nameof(product.Width), "Width cannot be negative", ErrorCode.ValidationError);
        }
        
        if (product.Height.HasValue && product.Height.Value < 0)
        {
            result.AddError(nameof(product.Height), "Height cannot be negative", ErrorCode.ValidationError);
        }
        
        var hasDimension = product.Length.HasValue || product.Width.HasValue || product.Height.HasValue;
        var hasAllDimensions = product.Length.HasValue && product.Width.HasValue && product.Height.HasValue;
        
        if (hasDimension && !hasAllDimensions)
        {
            result.AddWarning("Dimensions", "All dimensions (Length, Width, Height) should be provided together");
        }
    }
    
    /// <summary>
    /// Validates a calculation request
    /// </summary>
    public static ValidationResult ValidateCalculationRequest(Guid productId, int quantity, Dictionary<string, object> parameters)
    {
        var result = new ValidationResult { IsValid = true };
        
        if (productId == Guid.Empty)
        {
            result.AddError("ProductId", "Product ID is required", ErrorCode.ValidationError);
        }
        
        if (quantity <= 0)
        {
            result.AddError("Quantity", "Quantity must be greater than zero", ErrorCode.ValidationError);
        }
        
        if (quantity > 10000)
        {
            result.AddWarning("Quantity", "Large quantity may result in slower processing");
        }
        
        if (parameters == null || parameters.Count == 0)
        {
            result.AddWarning("Parameters", "No calculation parameters provided");
        }
        
        return result;
    }
    
    /// <summary>
    /// Validates a configuration model
    /// </summary>
    public static ValidationResult ValidateConfiguration(Configuration configuration)
    {
        var result = new ValidationResult { IsValid = true };
        
        if (string.IsNullOrWhiteSpace(configuration.Name))
        {
            result.AddError(nameof(configuration.Name), "Configuration name is required", ErrorCode.ValidationError);
        }
        
        if (string.IsNullOrWhiteSpace(configuration.Value))
        {
            result.AddError(nameof(configuration.Value), "Configuration value is required", ErrorCode.ValidationError);
        }
        
        if (configuration.EffectiveDate.HasValue && configuration.ExpirationDate.HasValue &&
            configuration.EffectiveDate.Value >= configuration.ExpirationDate.Value)
        {
            result.AddError("Dates", "Effective date must be before expiration date", ErrorCode.ValidationError);
        }
        
        return result;
    }
    
    /// <summary>
    /// Validates an email address format
    /// </summary>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        
        try
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Validates a SKU format
    /// </summary>
    public static bool IsValidSKU(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return false;
        
        // SKU should be alphanumeric with optional hyphens
        var regex = new Regex(@"^[A-Za-z0-9\-]+$");
        return regex.IsMatch(sku) && sku.Length <= 50;
    }
    
    /// <summary>
    /// Validates a numeric range
    /// </summary>
    public static bool IsInRange(decimal value, decimal min, decimal max)
    {
        return value >= min && value <= max;
    }
    
    /// <summary>
    /// Validates required fields in a dictionary
    /// </summary>
    public static ValidationResult ValidateRequiredFields(Dictionary<string, object> data, params string[] requiredFields)
    {
        var result = new ValidationResult { IsValid = true };
        
        foreach (var field in requiredFields)
        {
            if (!data.ContainsKey(field))
            {
                result.AddError(field, $"Required field '{field}' is missing", ErrorCode.ValidationError);
            }
            else if (data[field] == null)
            {
                result.AddError(field, $"Required field '{field}' cannot be null", ErrorCode.ValidationError);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Validates a date range
    /// </summary>
    public static bool IsValidDateRange(DateTime startDate, DateTime endDate)
    {
        return startDate <= endDate;
    }
    
    /// <summary>
    /// Checks if a string is alphanumeric
    /// </summary>
    public static bool IsAlphanumeric(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        
        return value.All(char.IsLetterOrDigit);
    }
}
