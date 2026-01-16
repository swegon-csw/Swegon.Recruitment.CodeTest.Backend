using Microsoft.AspNetCore.Mvc;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts;
using Swegon.Recruitment.CodeTest.Backend.Api.Services;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(ProductService productService, ILogger<ProductsController> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts(
        [FromQuery] string? search = null,
        [FromQuery] string? category = null,
        [FromQuery] string sortBy = "name",
        [FromQuery] string sortDirection = "asc"
    )
    {
        try
        {
            logger.LogInformation(
                "Getting products - Search: {Search}, Category: {Category}",
                search,
                category
            );

            var result = await productService.GetProductsAsync(
                search,
                category,
                sortBy,
                sortDirection
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting products");
            return StatusCode(500, new { error = "An error occurred while retrieving products" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDetailDto>> GetProduct(string id)
    {
        try
        {
            logger.LogInformation("Getting product with ID: {ProductId}", id);

            var product = await productService.GetProductByIdAsync(id);

            if (product == null)
            {
                logger.LogWarning("Product not found: {ProductId}", id);
                return NotFound(new { error = $"Product with ID {id} not found" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting product {ProductId}", id);
            return StatusCode(
                500,
                new { error = "An error occurred while retrieving the product" }
            );
        }
    }
}
