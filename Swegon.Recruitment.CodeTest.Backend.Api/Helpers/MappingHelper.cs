using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Helpers;

/// <summary>
/// Helper class for object mapping between domain models and DTOs
/// </summary>
public static class MappingHelper
{
    /// <summary>
    /// Maps a Product domain model to ProductResponse DTO
    /// </summary>
    public static ProductResponse MapToResponse(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Type = product.Type,
            Sku = product.SKU,
            Price = product.Price,
            IsActive = product.IsActive,
            Specifications = product.Specifications,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            CreatedBy = product.CreatedBy
        };
    }
    
    /// <summary>
    /// Maps a ProductRequest to Product domain model
    /// </summary>
    public static Product MapToProduct(ProductRequest request, string? createdBy = null)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        return new Product
        {
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            SKU = request.Sku,
            Price = request.Price,
            IsActive = request.IsActive,
            Specifications = request.Specifications ?? new Dictionary<string, string>(),
            CreatedBy = createdBy,
            UpdatedBy = createdBy
        };
    }
    
    /// <summary>
    /// Updates an existing Product from a ProductRequest
    /// </summary>
    public static void UpdateProductFromRequest(Product product, ProductRequest request, string? updatedBy = null)
    {
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(request);
        
        product.Name = request.Name;
        product.Description = request.Description;
        product.Type = request.Type;
        product.SKU = request.Sku;
        product.Price = request.Price;
        product.IsActive = request.IsActive;
        product.Specifications = request.Specifications ?? new Dictionary<string, string>();
        product.UpdatedAt = DateTime.UtcNow;
        product.UpdatedBy = updatedBy;
    }
    
    /// <summary>
    /// Maps a Calculation to CalculationResponse
    /// </summary>
    public static CalculationResponse MapToResponse(Calculation calculation)
    {
        ArgumentNullException.ThrowIfNull(calculation);
        
        return new CalculationResponse
        {
            Id = calculation.Id,
            ProductId = calculation.ProductId,
            Status = calculation.Status,
            Total = calculation.Total,
            Subtotal = calculation.Subtotal,
            DiscountAmount = calculation.DiscountAmount,
            TaxAmount = calculation.TaxAmount,
            Currency = calculation.Currency,
            Metadata = calculation.Metadata,
            CalculatedAt = calculation.CalculatedAt,
            Quantity = calculation.Quantity,
            Breakdown = calculation.Breakdown.Select(s => new Contracts.Responses.CalculationStepResponse
            {
                StepNumber = s.StepNumber,
                Description = s.Description,
                Value = s.Value,
                Formula = s.Formula
            }).ToList()
        };
    }
    
    /// <summary>
    /// Maps a CalculationResult to CalculationResponse
    /// </summary>
    public static CalculationResponse MapCalculationResultToResponse(CalculationResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        
        return new CalculationResponse
        {
            Id = result.CalculationId,
            ProductId = result.ProductId,
            Status = result.Status,
            Total = result.Total,
            Subtotal = result.Subtotal,
            DiscountAmount = result.DiscountAmount,
            TaxAmount = result.TaxAmount,
            Currency = result.Currency,
            Metadata = result.Metadata,
            CalculatedAt = result.CalculatedAt,
            Quantity = result.Quantity,
            Breakdown = result.Steps.Select(s => new Contracts.Responses.CalculationStepResponse
            {
                StepNumber = s.StepNumber,
                Description = s.Description,
                Value = s.Value,
                Formula = s.Formula
            }).ToList()
        };
    }
    
    /// <summary>
    /// Maps a Configuration to ConfigurationResponse
    /// </summary>
    public static ConfigurationResponse MapToResponse(Configuration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        
        return new ConfigurationResponse
        {
            Id = configuration.Id,
            Name = configuration.Name,
            Description = configuration.Description,
            Type = configuration.Type,
            Value = configuration.Value,
            Category = configuration.Category,
            IsActive = configuration.IsActive,
            CreatedAt = configuration.CreatedAt,
            UpdatedAt = configuration.UpdatedAt
        };
    }
    
    /// <summary>
    /// Maps a ValidationResult to ValidationResponse
    /// </summary>
    public static ValidationResponse MapToResponse(ValidationResult validationResult)
    {
        ArgumentNullException.ThrowIfNull(validationResult);
        
        return new ValidationResponse
        {
            IsValid = validationResult.IsValid,
            Errors = validationResult.Errors.Select(e => new Contracts.Responses.ValidationErrorResponse
            {
                Field = e.Field,
                Message = e.Message,
                ErrorCode = e.ErrorCode.ToString()
            }).ToList(),
            Warnings = validationResult.Warnings.Select(w => w.Message).ToList()
        };
    }
    
    /// <summary>
    /// Creates a paged response from a collection
    /// </summary>
    public static PagedResponse<T> CreatePagedResponse<T>(
        IEnumerable<T> items,
        int page,
        int pageSize,
        int totalCount)
    {
        return new PagedResponse<T>
        {
            Items = items.ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            HasNextPage = page * pageSize < totalCount,
            HasPreviousPage = page > 1
        };
    }
    
    /// <summary>
    /// Maps a collection of products to responses
    /// </summary>
    public static List<ProductResponse> MapToResponseList(IEnumerable<Product> products)
    {
        return products.Select(MapToResponse).ToList();
    }
    
    /// <summary>
    /// Maps a collection of calculations to responses
    /// </summary>
    public static List<CalculationResponse> MapToResponseList(IEnumerable<Calculation> calculations)
    {
        return calculations.Select(MapToResponse).ToList();
    }
}
