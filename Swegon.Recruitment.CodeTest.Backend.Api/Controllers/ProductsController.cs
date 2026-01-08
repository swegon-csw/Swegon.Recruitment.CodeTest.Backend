using Microsoft.AspNetCore.Mvc;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Services;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Controllers;

/// <summary>
/// Controller for managing product operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    /// <summary>
    /// Initializes a new instance of the ProductsController
    /// </summary>
    public ProductsController(
        ProductService productService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all products with optional filtering and pagination
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="productType">Optional product type filter</param>
    /// <param name="isActive">Optional active status filter</param>
    /// <param name="category">Optional category filter</param>
    /// <returns>List of products</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? productType = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? category = null)
    {
        try
        {
            _logger.LogInformation(
                "Getting products - Page: {Page}, PageSize: {PageSize}, Type: {Type}, Active: {Active}, Category: {Category}",
                page, pageSize, productType, isActive, category);

            var products = await _productService.GetProductsAsync(
                page,
                pageSize,
                productType,
                isActive,
                category);

            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return StatusCode(500, new { error = "An error occurred while retrieving products" });
        }
    }

    /// <summary>
    /// Gets a specific product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Product>> GetProduct(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting product with ID: {ProductId}", id);

            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                _logger.LogWarning("Product not found: {ProductId}", id);
                return NotFound(new { error = $"Product with ID {id} not found" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {ProductId}", id);
            return StatusCode(500, new { error = "An error occurred while retrieving the product" });
        }
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="product">Product to create</param>
    /// <returns>Created product</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new product: {ProductName}", product.Name);

            var result = await _productService.CreateProductAsync(product);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Product creation failed: {Error}", result.ErrorMessage);
                return BadRequest(new { error = result.ErrorMessage });
            }

            _logger.LogInformation("Product created successfully: {ProductId}", result.Data.Id);
            
            return CreatedAtAction(
                nameof(GetProduct),
                new { id = result.Data.Id },
                result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, new { error = "An error occurred while creating the product" });
        }
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="product">Updated product data</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Product>> UpdateProduct(Guid id, [FromBody] Product product)
    {
        try
        {
            if (id != product.Id)
            {
                return BadRequest(new { error = "ID in URL does not match ID in body" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating product: {ProductId}", id);

            var result = await _productService.UpdateProductAsync(product);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Product update failed: {Error}", result.ErrorMessage);
                
                if (result.ErrorMessage?.Contains("not found") == true)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }
                
                return BadRequest(new { error = result.ErrorMessage });
            }

            _logger.LogInformation("Product updated successfully: {ProductId}", id);
            
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return StatusCode(500, new { error = "An error occurred while updating the product" });
        }
    }

    /// <summary>
    /// Deletes a product (soft delete)
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting product: {ProductId}", id);

            var result = await _productService.DeleteProductAsync(id);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Product deletion failed: {Error}", result.ErrorMessage);
                return NotFound(new { error = result.ErrorMessage });
            }

            _logger.LogInformation("Product deleted successfully: {ProductId}", id);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return StatusCode(500, new { error = "An error occurred while deleting the product" });
        }
    }

    /// <summary>
    /// Searches for products by query string
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Matching products</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(
        [FromQuery] string query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { error = "Search query cannot be empty" });
            }

            _logger.LogInformation("Searching products with query: {Query}", query);

            var products = await _productService.SearchProductsAsync(query, page, pageSize);

            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with query: {Query}", query);
            return StatusCode(500, new { error = "An error occurred while searching products" });
        }
    }

    /// <summary>
    /// Gets products by category
    /// </summary>
    /// <param name="category">Category name</param>
    /// <returns>Products in category</returns>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(string category)
    {
        try
        {
            _logger.LogInformation("Getting products in category: {Category}", category);

            var products = await _productService.GetProductsAsync(
                page: 1,
                pageSize: 100,
                category: category);

            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products by category: {Category}", category);
            return StatusCode(500, new { error = "An error occurred while retrieving products" });
        }
    }

    /// <summary>
    /// Gets products that need reordering
    /// </summary>
    /// <returns>Products below reorder level</returns>
    [HttpGet("reorder")]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsNeedingReorder()
    {
        try
        {
            _logger.LogInformation("Getting products needing reorder");

            var allProducts = await _productService.GetProductsAsync(1, 1000);
            var productsNeedingReorder = allProducts.Where(p => p.NeedsReorder()).ToList();

            _logger.LogInformation("Found {Count} products needing reorder", productsNeedingReorder.Count);

            return Ok(productsNeedingReorder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products needing reorder");
            return StatusCode(500, new { error = "An error occurred while retrieving products" });
        }
    }
}
