using Swegon.Recruitment.CodeTest.Backend.Api.Contracts;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Services;

public class ProductService(JsonDataService jsonDataService)
{
    private List<ProductDetailDto>? _productsCache;

    public async Task<List<ProductDto>> GetProductsAsync(
        string? search = null,
        string? category = null,
        string sortBy = "name",
        string sortDirection = "asc"
    )
    {
        var products = await LoadProductsAsync();
        var filtered = products.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            filtered = filtered.Where(p =>
                p.Name.ToLower().Contains(searchLower)
                || p.Description.ToLower().Contains(searchLower)
            );
        }

        if (!string.IsNullOrWhiteSpace(category) && category != "all")
        {
            filtered = filtered.Where(p => p.Category == category);
        }

        filtered = sortBy.ToLower() switch
        {
            "price" => sortDirection == "desc"
                ? filtered.OrderByDescending(p => p.Price)
                : filtered.OrderBy(p => p.Price),
            "rating" => sortDirection == "desc"
                ? filtered.OrderByDescending(p => p.Rating ?? 0)
                : filtered.OrderBy(p => p.Rating ?? 0),
            _ => sortDirection == "desc"
                ? filtered.OrderByDescending(p => p.Name)
                : filtered.OrderBy(p => p.Name),
        };

        return filtered
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Category = p.Category,
                Price = p.Price,
                Currency = p.Currency,
                ImageUrl = p.ImageUrl,
                InStock = p.InStock,
                Rating = p.Rating,
                ReviewCount = p.ReviewCount,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
            })
            .ToList();
    }

    public async Task<ProductDetailDto?> GetProductByIdAsync(string id)
    {
        var products = await LoadProductsAsync();
        return products.FirstOrDefault(p => p.Id == id);
    }

    private async Task<List<ProductDetailDto>> LoadProductsAsync()
    {
        if (_productsCache != null)
            return _productsCache;

        _productsCache = await jsonDataService.LoadDataAsync<List<ProductDetailDto>>(
            "products.json"
        );
        return _productsCache;
    }
}
