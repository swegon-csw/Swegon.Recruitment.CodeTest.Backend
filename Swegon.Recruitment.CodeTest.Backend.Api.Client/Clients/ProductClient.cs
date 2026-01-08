using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Swegon.Recruitment.CodeTest.Backend.Api.Client.Configuration;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Client.Clients;

/// <summary>
/// Interface for product client operations.
/// </summary>
public interface IProductClient
{
    Task<PagedResponse<ProductResponse>> GetProductsAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<ProductResponse> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductResponse> CreateProductAsync(ProductRequest request, CancellationToken cancellationToken = default);
    Task<ProductResponse> UpdateProductAsync(Guid id, ProductRequest request, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SearchResponse<ProductResponse>> SearchProductsAsync(SearchRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Client for interacting with the Products API.
/// </summary>
public class ProductClient : BaseClient, IProductClient
{
    private const string BaseEndpoint = "api/products";

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="configuration">The client configuration.</param>
    public ProductClient(HttpClient httpClient, ClientConfiguration configuration)
        : base(httpClient, configuration)
    {
    }

    /// <summary>
    /// Gets a paginated list of products.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of products.</returns>
    public async Task<PagedResponse<ProductResponse>> GetProductsAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0.");
        }

        if (pageSize < 1 || pageSize > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be between 1 and 100.");
        }

        var endpoint = $"{BaseEndpoint}?page={page}&pageSize={pageSize}";
        return await GetAsync<PagedResponse<ProductResponse>>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a product by its unique identifier.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The product details.</returns>
    public async Task<ProductResponse> GetProductByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(id));
        }

        var endpoint = $"{BaseEndpoint}/{id}";
        return await GetAsync<ProductResponse>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="request">The product creation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created product.</returns>
    public async Task<ProductResponse> CreateProductAsync(
        ProductRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return await PostAsync<ProductRequest, ProductResponse>(
            BaseEndpoint,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="request">The product update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated product.</returns>
    public async Task<ProductResponse> UpdateProductAsync(
        Guid id,
        ProductRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(id));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var endpoint = $"{BaseEndpoint}/{id}";
        return await PutAsync<ProductRequest, ProductResponse>(
            endpoint,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a product.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(id));
        }

        var endpoint = $"{BaseEndpoint}/{id}";
        await DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Searches for products based on search criteria.
    /// </summary>
    /// <param name="request">The search request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Search results containing matching products.</returns>
    public async Task<SearchResponse<ProductResponse>> SearchProductsAsync(
        SearchRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var endpoint = $"{BaseEndpoint}/search";
        return await PostAsync<SearchRequest, SearchResponse<ProductResponse>>(
            endpoint,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets products by category.
    /// </summary>
    /// <param name="category">The category name.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of products in the category.</returns>
    public async Task<PagedResponse<ProductResponse>> GetProductsByCategoryAsync(
        string category,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("Category cannot be null or empty.", nameof(category));
        }

        var endpoint = $"{BaseEndpoint}/category/{Uri.EscapeDataString(category)}?page={page}&pageSize={pageSize}";
        return await GetAsync<PagedResponse<ProductResponse>>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets active products only.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of active products.</returns>
    public async Task<PagedResponse<ProductResponse>> GetActiveProductsAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"{BaseEndpoint}/active?page={page}&pageSize={pageSize}";
        return await GetAsync<PagedResponse<ProductResponse>>(endpoint, cancellationToken).ConfigureAwait(false);
    }
}
