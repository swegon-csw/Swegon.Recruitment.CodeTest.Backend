using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Validators;

/// <summary>
/// Validator for product-related operations
/// </summary>
public class ProductValidator
{
    /// <summary>
    /// Validates a product model
    /// </summary>
    public ValidationResult Validate(Product product)
    {
        var result = new ValidationResult { IsValid = true, ValidatorName = nameof(ProductValidator) };
        
        ValidateName(product, result);
        ValidatePrice(product, result);
        ValidateSKU(product, result);
        ValidateDescription(product, result);
        ValidateDimensions(product, result);
        ValidateInventory(product, result);
        ValidateSpecifications(product, result);
        
        return result;
    }
    
    /// <summary>
    /// Validates a product request
    /// </summary>
    public ValidationResult ValidateRequest(ProductRequest request)
    {
        var result = new ValidationResult { IsValid = true, ValidatorName = nameof(ProductValidator) };
        
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            result.AddError(nameof(request.Name), "Product name is required", ErrorCode.ValidationError);
        }
        else if (request.Name.Length < 3)
        {
            result.AddError(nameof(request.Name), "Product name must be at least 3 characters long", ErrorCode.ValidationError);
        }
        else if (request.Name.Length > 100)
        {
            result.AddError(nameof(request.Name), "Product name cannot exceed 100 characters", ErrorCode.ValidationError);
        }
        
        if (request.Price < 0)
        {
            result.AddError(nameof(request.Price), "Product price cannot be negative", ErrorCode.ValidationError);
        }
        else if (request.Price == 0)
        {
            result.AddWarning(nameof(request.Price), "Product price is zero");
        }
        else if (request.Price > 1000000)
        {
            result.AddWarning(nameof(request.Price), "Product price is very high");
        }
        
        if (!string.IsNullOrEmpty(request.Sku))
        {
            if (request.Sku.Length > 50)
            {
                result.AddError(nameof(request.Sku), "SKU cannot exceed 50 characters", ErrorCode.ValidationError);
            }
            
            if (!System.Text.RegularExpressions.Regex.IsMatch(request.Sku, @"^[A-Za-z0-9\-]+$"))
            {
                result.AddError(nameof(request.Sku), "SKU can only contain alphanumeric characters and hyphens", ErrorCode.ValidationError);
            }
        }
        
        if (request.Description?.Length > 500)
        {
            result.AddError(nameof(request.Description), "Description cannot exceed 500 characters", ErrorCode.ValidationError);
        }
        
        if (request.Specifications != null && request.Specifications.Count > 50)
        {
            result.AddWarning(nameof(request.Specifications), "Large number of specifications may affect performance");
        }
        
        return result;
    }
    
    /// <summary>
    /// Validates product name
    /// </summary>
    private void ValidateName(Product product, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
        {
            result.AddError(nameof(product.Name), "Product name is required", ErrorCode.ValidationError);
            return;
        }
        
        if (product.Name.Length < 3)
        {
            result.AddError(nameof(product.Name), "Product name must be at least 3 characters long", ErrorCode.ValidationError);
        }
        
        if (product.Name.Length > 100)
        {
            result.AddError(nameof(product.Name), "Product name cannot exceed 100 characters", ErrorCode.ValidationError);
        }
        
        if (product.Name.All(char.IsDigit))
        {
            result.AddWarning(nameof(product.Name), "Product name should not consist only of numbers");
        }
    }
    
    /// <summary>
    /// Validates product price
    /// </summary>
    private void ValidatePrice(Product product, ValidationResult result)
    {
        if (product.Price < 0)
        {
            result.AddError(nameof(product.Price), "Product price cannot be negative", ErrorCode.ValidationError);
        }
        
        if (product.Price == 0 && product.IsActive)
        {
            result.AddWarning(nameof(product.Price), "Active product has zero price");
        }
        
        if (product.Price > 1000000)
        {
            result.AddWarning(nameof(product.Price), "Product price is unusually high");
        }
    }
    
    /// <summary>
    /// Validates product SKU
    /// </summary>
    private void ValidateSKU(Product product, ValidationResult result)
    {
        if (string.IsNullOrEmpty(product.SKU))
        {
            result.AddWarning(nameof(product.SKU), "Product SKU is not set");
            return;
        }
        
        if (product.SKU.Length > 50)
        {
            result.AddError(nameof(product.SKU), "SKU cannot exceed 50 characters", ErrorCode.ValidationError);
        }
        
        if (!System.Text.RegularExpressions.Regex.IsMatch(product.SKU, @"^[A-Za-z0-9\-]+$"))
        {
            result.AddError(nameof(product.SKU), "SKU can only contain alphanumeric characters and hyphens", ErrorCode.ValidationError);
        }
    }
    
    /// <summary>
    /// Validates product description
    /// </summary>
    private void ValidateDescription(Product product, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(product.Description) && product.Type == ProductType.Custom)
        {
            result.AddWarning(nameof(product.Description), "Custom products should have a description");
        }
        
        if (product.Description?.Length > 500)
        {
            result.AddError(nameof(product.Description), "Description cannot exceed 500 characters", ErrorCode.ValidationError);
        }
    }
    
    /// <summary>
    /// Validates product dimensions
    /// </summary>
    private void ValidateDimensions(Product product, ValidationResult result)
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
        
        if (product.Weight.HasValue && product.Weight.Value < 0)
        {
            result.AddError(nameof(product.Weight), "Weight cannot be negative", ErrorCode.ValidationError);
        }
        
        var hasSomeDimensions = product.Length.HasValue || product.Width.HasValue || product.Height.HasValue;
        var hasAllDimensions = product.Length.HasValue && product.Width.HasValue && product.Height.HasValue;
        
        if (hasSomeDimensions && !hasAllDimensions)
        {
            result.AddWarning("Dimensions", "All dimensions (Length, Width, Height) should be provided together");
        }
        
        if (hasAllDimensions)
        {
            var volume = product.CalculateVolume();
            if (volume.HasValue && volume.Value > 10000000) // Very large volume
            {
                result.AddWarning("Dimensions", "Product has unusually large volume");
            }
        }
    }
    
    /// <summary>
    /// Validates inventory information
    /// </summary>
    private void ValidateInventory(Product product, ValidationResult result)
    {
        if (product.StockQuantity < 0)
        {
            result.AddError(nameof(product.StockQuantity), "Stock quantity cannot be negative", ErrorCode.ValidationError);
        }
        
        if (product.ReorderLevel < 0)
        {
            result.AddError(nameof(product.ReorderLevel), "Reorder level cannot be negative", ErrorCode.ValidationError);
        }
        
        if (product.StockQuantity > 0 && product.StockQuantity <= product.ReorderLevel && product.IsActive)
        {
            result.AddWarning("Inventory", "Stock is at or below reorder level");
        }
        
        if (product.StockQuantity == 0 && product.IsActive)
        {
            result.AddWarning("Inventory", "Active product is out of stock");
        }
        
        if (product.ReorderLevel > product.StockQuantity * 2)
        {
            result.AddWarning(nameof(product.ReorderLevel), "Reorder level is much higher than current stock");
        }
    }
    
    /// <summary>
    /// Validates product specifications
    /// </summary>
    private void ValidateSpecifications(Product product, ValidationResult result)
    {
        if (product.Specifications == null || product.Specifications.Count == 0)
        {
            if (product.Type == ProductType.Custom || product.Type == ProductType.Industrial)
            {
                result.AddWarning(nameof(product.Specifications), "Products of this type typically have specifications");
            }
            return;
        }
        
        if (product.Specifications.Count > 50)
        {
            result.AddWarning(nameof(product.Specifications), "Large number of specifications may affect performance");
        }
        
        foreach (var spec in product.Specifications)
        {
            if (string.IsNullOrWhiteSpace(spec.Key))
            {
                result.AddError(nameof(product.Specifications), "Specification keys cannot be empty", ErrorCode.ValidationError);
            }
            
            if (spec.Key.Length > 100)
            {
                result.AddError(nameof(product.Specifications), "Specification key is too long", ErrorCode.ValidationError);
            }
            
            if (spec.Value?.Length > 500)
            {
                result.AddError(nameof(product.Specifications), "Specification value is too long", ErrorCode.ValidationError);
            }
        }
    }
    
    /// <summary>
    /// Validates product for activation
    /// </summary>
    public ValidationResult ValidateForActivation(Product product)
    {
        var result = Validate(product);
        
        if (product.Price == 0)
        {
            result.AddError(nameof(product.Price), "Cannot activate product with zero price", ErrorCode.ValidationError);
        }
        
        if (string.IsNullOrEmpty(product.SKU))
        {
            result.AddError(nameof(product.SKU), "Cannot activate product without SKU", ErrorCode.ValidationError);
        }
        
        if (product.StockQuantity == 0)
        {
            result.AddWarning("Inventory", "Activating product with no stock");
        }
        
        return result;
    }
}
