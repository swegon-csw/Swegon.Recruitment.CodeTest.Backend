using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Interfaces;

/// <summary>
/// Interface for product service operations
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Gets all products with optional filtering
    /// </summary>
    Task<PagedResponse<ProductResponse>> GetProductsAsync(FilterRequest filter, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a product by ID
    /// </summary>
    Task<ProductResponse?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new product
    /// </summary>
    Task<ProductResponse> CreateProductAsync(ProductRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing product
    /// </summary>
    Task<ProductResponse> UpdateProductAsync(Guid id, ProductRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a product
    /// </summary>
    Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Searches for products
    /// </summary>
    Task<SearchResponse<ProductResponse>> SearchProductsAsync(SearchRequest request, CancellationToken cancellationToken = default);
}
