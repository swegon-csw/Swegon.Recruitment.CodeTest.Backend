using System.Collections.Concurrent;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Validators;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Services;

/// <summary>
/// Service for managing product operations with in-memory storage
/// </summary>
public class ProductService
{
    private readonly ConcurrentDictionary<Guid, Product> _products;
    private readonly ProductValidator _validator;
    private readonly CacheService _cacheService;
    private readonly ILogger<ProductService> _logger;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private bool _isInitialized = false;

    /// <summary>
    /// Initializes a new instance of the ProductService
    /// </summary>
    public ProductService(
        ProductValidator validator,
        CacheService cacheService,
        ILogger<ProductService> logger)
    {
        _products = new ConcurrentDictionary<Guid, Product>();
        _validator = validator;
        _cacheService = cacheService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all products with optional filtering and pagination
    /// </summary>
    public async Task<IEnumerable<Product>> GetProductsAsync(
        int page = 1,
        int pageSize = 20,
        string? productType = null,
        bool? isActive = null,
        string? category = null)
    {
        await EnsureInitializedAsync();

        var cacheKey = $"products_p{page}_ps{pageSize}_t{productType}_a{isActive}_c{category}";
        
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                _logger.LogDebug("Fetching products from storage");

                var query = _products.Values.Where(p => !p.IsDeleted);

                // Apply filters
                if (!string.IsNullOrEmpty(productType) && Enum.TryParse<ProductType>(productType, true, out var type))
                {
                    query = query.Where(p => p.Type == type);
                }

                if (isActive.HasValue)
                {
                    query = query.Where(p => p.IsActive == isActive.Value);
                }

                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(p => p.Category?.Equals(category, StringComparison.OrdinalIgnoreCase) == true);
                }

                // Apply pagination
                var products = query
                    .OrderBy(p => p.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return await Task.FromResult(products.AsEnumerable());
            },
            TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Gets a product by ID
    /// </summary>
    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        await EnsureInitializedAsync();

        var cacheKey = $"product_{id}";
        
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                _logger.LogDebug("Fetching product {ProductId} from storage", id);
                
                if (_products.TryGetValue(id, out var product) && !product.IsDeleted)
                {
                    return await Task.FromResult(product);
                }
                
                return await Task.FromResult<Product?>(null);
            },
            TimeSpan.FromMinutes(10));
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    public async Task<Result<Product>> CreateProductAsync(Product product)
    {
        await EnsureInitializedAsync();

        try
        {
            // Validate the product
            var validationResult = await _validator.ValidateAsync(product);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors);
                _logger.LogWarning("Product validation failed: {Errors}", errors);
                return Result<Product>.Failure($"Validation failed: {errors}");
            }

            // Ensure unique ID
            if (product.Id == Guid.Empty)
            {
                product.Id = Guid.NewGuid();
            }

            // Check if product with same SKU already exists
            if (_products.Values.Any(p => !p.IsDeleted && p.SKU == product.SKU))
            {
                _logger.LogWarning("Product with SKU {SKU} already exists", product.SKU);
                return Result<Product>.Failure($"Product with SKU '{product.SKU}' already exists");
            }

            // Set timestamps
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            // Add to storage
            if (!_products.TryAdd(product.Id, product))
            {
                _logger.LogError("Failed to add product {ProductId} to storage", product.Id);
                return Result<Product>.Failure("Failed to create product");
            }

            // Invalidate cache
            await InvalidateProductCacheAsync();

            _logger.LogInformation("Created product {ProductId} with SKU {SKU}", product.Id, product.SKU);
            
            return Result<Product>.Success(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return Result<Product>.Failure($"Error creating product: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    public async Task<Result<Product>> UpdateProductAsync(Product product)
    {
        await EnsureInitializedAsync();

        try
        {
            // Validate the product
            var validationResult = await _validator.ValidateAsync(product);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors);
                _logger.LogWarning("Product validation failed: {Errors}", errors);
                return Result<Product>.Failure($"Validation failed: {errors}");
            }

            // Check if product exists
            if (!_products.TryGetValue(product.Id, out var existingProduct))
            {
                _logger.LogWarning("Product {ProductId} not found", product.Id);
                return Result<Product>.Failure($"Product with ID '{product.Id}' not found");
            }

            if (existingProduct.IsDeleted)
            {
                _logger.LogWarning("Product {ProductId} is deleted", product.Id);
                return Result<Product>.Failure($"Product with ID '{product.Id}' has been deleted");
            }

            // Check if SKU is being changed to one that already exists
            if (product.SKU != existingProduct.SKU &&
                _products.Values.Any(p => !p.IsDeleted && p.Id != product.Id && p.SKU == product.SKU))
            {
                _logger.LogWarning("Product with SKU {SKU} already exists", product.SKU);
                return Result<Product>.Failure($"Product with SKU '{product.SKU}' already exists");
            }

            // Preserve creation timestamp
            product.CreatedAt = existingProduct.CreatedAt;
            product.UpdatedAt = DateTime.UtcNow;

            // Update in storage
            _products[product.Id] = product;

            // Invalidate cache
            await InvalidateProductCacheAsync();
            _cacheService.Remove($"product_{product.Id}");

            _logger.LogInformation("Updated product {ProductId}", product.Id);
            
            return Result<Product>.Success(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", product.Id);
            return Result<Product>.Failure($"Error updating product: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a product (soft delete)
    /// </summary>
    public async Task<Result<bool>> DeleteProductAsync(Guid id)
    {
        await EnsureInitializedAsync();

        try
        {
            if (!_products.TryGetValue(id, out var product))
            {
                _logger.LogWarning("Product {ProductId} not found", id);
                return Result<bool>.Failure($"Product with ID '{id}' not found");
            }

            if (product.IsDeleted)
            {
                _logger.LogWarning("Product {ProductId} is already deleted", id);
                return Result<bool>.Failure($"Product with ID '{id}' is already deleted");
            }

            // Soft delete
            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            _products[id] = product;

            // Invalidate cache
            await InvalidateProductCacheAsync();
            _cacheService.Remove($"product_{id}");

            _logger.LogInformation("Deleted product {ProductId}", id);
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return Result<bool>.Failure($"Error deleting product: {ex.Message}");
        }
    }

    /// <summary>
    /// Searches for products by query string
    /// </summary>
    public async Task<IEnumerable<Product>> SearchProductsAsync(string query, int page = 1, int pageSize = 20)
    {
        await EnsureInitializedAsync();

        var cacheKey = $"search_{query}_p{page}_ps{pageSize}";
        
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                _logger.LogDebug("Searching products with query: {Query}", query);

                var lowerQuery = query.ToLowerInvariant();
                
                var results = _products.Values
                    .Where(p => !p.IsDeleted)
                    .Select(p => new
                    {
                        Product = p,
                        Score = CalculateSearchScore(p, lowerQuery)
                    })
                    .Where(x => x.Score > 0)
                    .OrderByDescending(x => x.Score)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => x.Product)
                    .ToList();

                return await Task.FromResult(results.AsEnumerable());
            },
            TimeSpan.FromMinutes(2));
    }

    /// <summary>
    /// Initializes sample products if storage is empty
    /// </summary>
    private async Task EnsureInitializedAsync()
    {
        if (_isInitialized) return;

        await _initializationLock.WaitAsync();
        try
        {
            if (_isInitialized) return;

            if (_products.IsEmpty)
            {
                await InitializeSampleProducts();
            }

            _isInitialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    /// <summary>
    /// Initializes sample products for testing
    /// </summary>
    private async Task InitializeSampleProducts()
    {
        _logger.LogInformation("Initializing sample products");

        var sampleProducts = new[]
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Standard Air Handler",
                Description = "Standard air handling unit with basic features",
                Type = ProductType.Standard,
                SKU = "AHU-STD-001",
                Price = 5000.00m,
                IsActive = true,
                Specifications = new Dictionary<string, object>
                {
                    ["airflow"] = "2000 m³/h",
                    ["efficiency"] = 0.85,
                    ["noiseLevel"] = 45
                },
                Weight = 250.0,
                Length = 1500.0,
                Width = 800.0,
                Height = 1200.0,
                Manufacturer = "Swegon",
                StockQuantity = 50,
                ReorderLevel = 10,
                Category = "Air Handlers",
                Tags = new List<string> { "standard", "air-handler", "hvac" },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Premium Air Handler",
                Description = "Premium air handling unit with advanced features and controls",
                Type = ProductType.Premium,
                SKU = "AHU-PRM-001",
                Price = 12000.00m,
                IsActive = true,
                Specifications = new Dictionary<string, object>
                {
                    ["airflow"] = "5000 m³/h",
                    ["efficiency"] = 0.95,
                    ["noiseLevel"] = 35,
                    ["hasHeatRecovery"] = true,
                    ["hasHumidityControl"] = true
                },
                Weight = 450.0,
                Length = 2200.0,
                Width = 1200.0,
                Height = 1500.0,
                Manufacturer = "Swegon",
                StockQuantity = 25,
                ReorderLevel = 5,
                Category = "Air Handlers",
                Tags = new List<string> { "premium", "air-handler", "hvac", "heat-recovery" },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Custom Air Handler",
                Description = "Fully customizable air handling unit built to specification",
                Type = ProductType.Custom,
                SKU = "AHU-CST-001",
                Price = 25000.00m,
                IsActive = true,
                Specifications = new Dictionary<string, object>
                {
                    ["customizable"] = true,
                    ["minAirflow"] = "1000 m³/h",
                    ["maxAirflow"] = "15000 m³/h",
                    ["efficiency"] = 0.98,
                    ["noiseLevel"] = 30
                },
                Weight = 800.0,
                Length = 3000.0,
                Width = 1800.0,
                Height = 2000.0,
                Manufacturer = "Swegon",
                StockQuantity = 5,
                ReorderLevel = 2,
                Category = "Air Handlers",
                Tags = new List<string> { "custom", "air-handler", "hvac", "high-performance" },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Industrial Air Handler",
                Description = "Heavy-duty industrial air handling system",
                Type = ProductType.Industrial,
                SKU = "AHU-IND-001",
                Price = 45000.00m,
                IsActive = true,
                Specifications = new Dictionary<string, object>
                {
                    ["airflow"] = "20000 m³/h",
                    ["efficiency"] = 0.92,
                    ["noiseLevel"] = 55,
                    ["heavyDuty"] = true,
                    ["operatingTemperature"] = "-20 to 50°C"
                },
                Weight = 1500.0,
                Length = 4000.0,
                Width = 2500.0,
                Height = 2500.0,
                Manufacturer = "Swegon",
                StockQuantity = 3,
                ReorderLevel = 1,
                Category = "Air Handlers",
                Tags = new List<string> { "industrial", "air-handler", "hvac", "heavy-duty" },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        foreach (var product in sampleProducts)
        {
            _products.TryAdd(product.Id, product);
        }

        _logger.LogInformation("Initialized {Count} sample products", sampleProducts.Length);
        
        await Task.CompletedTask;
    }

    /// <summary>
    /// Calculates search relevance score
    /// </summary>
    private int CalculateSearchScore(Product product, string query)
    {
        var score = 0;

        // Exact name match
        if (product.Name?.ToLowerInvariant() == query)
            score += 100;
        // Name contains query
        else if (product.Name?.ToLowerInvariant().Contains(query) == true)
            score += 50;

        // SKU match
        if (product.SKU?.ToLowerInvariant() == query)
            score += 80;
        else if (product.SKU?.ToLowerInvariant().Contains(query) == true)
            score += 40;

        // Description contains query
        if (product.Description?.ToLowerInvariant().Contains(query) == true)
            score += 20;

        // Category match
        if (product.Category?.ToLowerInvariant() == query)
            score += 30;
        else if (product.Category?.ToLowerInvariant().Contains(query) == true)
            score += 15;

        // Tags match
        if (product.Tags?.Any(t => t.ToLowerInvariant() == query) == true)
            score += 25;
        else if (product.Tags?.Any(t => t.ToLowerInvariant().Contains(query)) == true)
            score += 10;

        // Manufacturer match
        if (product.Manufacturer?.ToLowerInvariant().Contains(query) == true)
            score += 10;

        return score;
    }

    /// <summary>
    /// Invalidates product list cache
    /// </summary>
    private async Task InvalidateProductCacheAsync()
    {
        // In a real implementation, you might want to use a pattern to remove all related cache entries
        // For now, we'll just remove known cache key patterns
        _cacheService.Remove("products_");
        _cacheService.Remove("search_");
        
        await Task.CompletedTask;
    }
}
