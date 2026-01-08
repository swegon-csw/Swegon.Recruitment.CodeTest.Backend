using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Services;
using Swegon.Recruitment.CodeTest.Backend.Api.Validators;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.TestData;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.Utilities;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Unit.Services;

/// <summary>
/// Unit tests for ProductService
/// </summary>
public class ProductServiceTests : IDisposable
{
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly Mock<ILogger<CacheService>> _mockCacheLogger;
    private readonly Mock<ProductValidator> _mockValidator;
    private readonly CacheService _cacheService;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockLogger = MockHelper.CreateMockLogger<ProductService>();
        _mockCacheLogger = MockHelper.CreateMockLogger<CacheService>();
        _mockValidator = new Mock<ProductValidator>();
        
        var memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(
            new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
        _cacheService = new CacheService(memoryCache, _mockCacheLogger.Object);
        
        _productService = new ProductService(
            _mockValidator.Object,
            _cacheService,
            _mockLogger.Object);
    }

    #region GetAll Tests

    /// <summary>
    /// GetProductsAsync should return all products when no filters applied
    /// </summary>
    [Fact]
    public async Task GetProductsAsync_NoFilters_ReturnsAllProducts()
    {
        // Arrange
        var product1 = await _productService.CreateProductAsync(ProductTestDataBuilder.CreateStandardProduct());
        var product2 = await _productService.CreateProductAsync(ProductTestDataBuilder.CreatePremiumProduct());

        // Act
        var result = await _productService.GetProductsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterOrEqualTo(2);
        result.Should().Contain(p => p.Id == product1.Id);
        result.Should().Contain(p => p.Id == product2.Id);
    }

    /// <summary>
    /// GetProductsAsync should filter by product type
    /// </summary>
    [Theory]
    [InlineData(ProductType.Standard)]
    [InlineData(ProductType.Premium)]
    [InlineData(ProductType.Industrial)]
    public async Task GetProductsAsync_FilterByType_ReturnsMatchingProducts(ProductType type)
    {
        // Arrange
        var product = new ProductTestDataBuilder().WithType(type).Build();
        await _productService.CreateProductAsync(product);

        // Act
        var result = await _productService.GetProductsAsync(productType: type.ToString());

        // Assert
        result.Should().NotBeNull();
        result.Should().OnlyContain(p => p.Type == type);
    }

    /// <summary>
    /// GetProductsAsync should filter by active status
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetProductsAsync_FilterByActiveStatus_ReturnsMatchingProducts(bool isActive)
    {
        // Arrange
        var product = new ProductTestDataBuilder().WithIsActive(isActive).Build();
        await _productService.CreateProductAsync(product);

        // Act
        var result = await _productService.GetProductsAsync(isActive: isActive);

        // Assert
        result.Should().NotBeNull();
        result.Should().OnlyContain(p => p.IsActive == isActive);
    }

    /// <summary>
    /// GetProductsAsync should apply pagination correctly
    /// </summary>
    [Fact]
    public async Task GetProductsAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        for (int i = 0; i < 10; i++)
        {
            var product = new ProductTestDataBuilder()
                .WithName($"Product {i}")
                .Build();
            await _productService.CreateProductAsync(product);
        }

        // Act
        var page1 = await _productService.GetProductsAsync(page: 1, pageSize: 5);
        var page2 = await _productService.GetProductsAsync(page: 2, pageSize: 5);

        // Assert
        page1.Should().HaveCount(5);
        page2.Should().HaveCount(5);
        page1.Select(p => p.Id).Should().NotIntersectWith(page2.Select(p => p.Id));
    }

    /// <summary>
    /// GetProductsAsync should filter by category
    /// </summary>
    [Fact]
    public async Task GetProductsAsync_FilterByCategory_ReturnsMatchingProducts()
    {
        // Arrange
        var category = "TestCategory";
        var product = new ProductTestDataBuilder()
            .WithCategory(category)
            .Build();
        await _productService.CreateProductAsync(product);

        // Act
        var result = await _productService.GetProductsAsync(category: category);

        // Assert
        result.Should().NotBeNull();
        result.Should().OnlyContain(p => p.Category == category);
    }

    #endregion

    #region GetById Tests

    /// <summary>
    /// GetProductByIdAsync should return product when exists
    /// </summary>
    [Fact]
    public async Task GetProductByIdAsync_ValidId_ReturnsProduct()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var created = await _productService.CreateProductAsync(product);

        // Act
        var result = await _productService.GetProductByIdAsync(created.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.Name.Should().Be(created.Name);
    }

    /// <summary>
    /// GetProductByIdAsync should return null when product not found
    /// </summary>
    [Fact]
    public async Task GetProductByIdAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _productService.GetProductByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// GetProductByIdAsync should throw when ID is empty
    /// </summary>
    [Fact]
    public async Task GetProductByIdAsync_EmptyGuid_ThrowsException()
    {
        // Arrange
        var emptyId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _productService.GetProductByIdAsync(emptyId));
    }

    #endregion

    #region Create Tests

    /// <summary>
    /// CreateProductAsync should create product successfully
    /// </summary>
    [Fact]
    public async Task CreateProductAsync_ValidProduct_CreatesSuccessfully()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        _mockValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult { IsValid = true });

        // Act
        var result = await _productService.CreateProductAsync(product);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.Name.Should().Be(product.Name);
        TestHelper.AssertRecentDate(result.CreatedAt);
    }

    /// <summary>
    /// CreateProductAsync should throw when product is null
    /// </summary>
    [Fact]
    public async Task CreateProductAsync_NullProduct_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _productService.CreateProductAsync(null!));
    }

    /// <summary>
    /// CreateProductAsync should validate product before creating
    /// </summary>
    [Fact]
    public async Task CreateProductAsync_InvalidProduct_ThrowsException()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        _mockValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult 
            { 
                IsValid = false, 
                Errors = new[] { "Name is required" }
            });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _productService.CreateProductAsync(product));
    }

    /// <summary>
    /// CreateProductAsync should set CreatedAt and UpdatedAt timestamps
    /// </summary>
    [Fact]
    public async Task CreateProductAsync_SetsTimestamps()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        _mockValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult { IsValid = true });

        // Act
        var result = await _productService.CreateProductAsync(product);

        // Assert
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    /// <summary>
    /// CreateProductAsync should create products with different types
    /// </summary>
    [Theory]
    [InlineData(ProductType.Standard, 100.00)]
    [InlineData(ProductType.Premium, 500.00)]
    [InlineData(ProductType.Industrial, 1000.00)]
    public async Task CreateProductAsync_DifferentTypes_CreatesSuccessfully(ProductType type, decimal price)
    {
        // Arrange
        var product = new ProductTestDataBuilder()
            .WithType(type)
            .WithPrice(price)
            .Build();
        _mockValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult { IsValid = true });

        // Act
        var result = await _productService.CreateProductAsync(product);

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(type);
        result.Price.Should().Be(price);
    }

    #endregion

    #region Update Tests

    /// <summary>
    /// UpdateProductAsync should update existing product
    /// </summary>
    [Fact]
    public async Task UpdateProductAsync_ValidProduct_UpdatesSuccessfully()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        _mockValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult { IsValid = true });
        var created = await _productService.CreateProductAsync(product);
        
        created.Name = "Updated Name";
        created.Price = 200.00m;

        // Act
        var result = await _productService.UpdateProductAsync(created.Id, created);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        result.Price.Should().Be(200.00m);
        result.UpdatedAt.Should().BeAfter(result.CreatedAt);
    }

    /// <summary>
    /// UpdateProductAsync should return null when product not found
    /// </summary>
    [Fact]
    public async Task UpdateProductAsync_NonExistentProduct_ReturnsNull()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _productService.UpdateProductAsync(nonExistentId, product);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// UpdateProductAsync should validate before updating
    /// </summary>
    [Fact]
    public async Task UpdateProductAsync_InvalidProduct_ThrowsException()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        _mockValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult { IsValid = true });
        var created = await _productService.CreateProductAsync(product);
        
        _mockValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult 
            { 
                IsValid = false, 
                Errors = new[] { "Invalid data" }
            });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _productService.UpdateProductAsync(created.Id, created));
    }

    #endregion

    #region Delete Tests

    /// <summary>
    /// DeleteProductAsync should soft delete product
    /// </summary>
    [Fact]
    public async Task DeleteProductAsync_ValidId_SoftDeletesProduct()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        _mockValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult { IsValid = true });
        var created = await _productService.CreateProductAsync(product);

        // Act
        var result = await _productService.DeleteProductAsync(created.Id);

        // Assert
        result.Should().BeTrue();
        
        // Verify product is marked as deleted
        var deleted = await _productService.GetProductByIdAsync(created.Id);
        deleted.Should().BeNull();
    }

    /// <summary>
    /// DeleteProductAsync should return false when product not found
    /// </summary>
    [Fact]
    public async Task DeleteProductAsync_NonExistentProduct_ReturnsFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _productService.DeleteProductAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Search Tests

    /// <summary>
    /// SearchProductsAsync should find products by name
    /// </summary>
    [Fact]
    public async Task SearchProductsAsync_ByName_ReturnsMatchingProducts()
    {
        // Arrange
        var searchTerm = "Special";
        var product = new ProductTestDataBuilder()
            .WithName($"Special Product 123")
            .Build();
        _mockValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult { IsValid = true });
        await _productService.CreateProductAsync(product);

        // Act
        var result = await _productService.SearchProductsAsync(searchTerm);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// SearchProductsAsync should return empty when no matches
    /// </summary>
    [Fact]
    public async Task SearchProductsAsync_NoMatches_ReturnsEmpty()
    {
        // Arrange
        var searchTerm = "NonExistentProduct12345";

        // Act
        var result = await _productService.SearchProductsAsync(searchTerm);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// SearchProductsAsync should be case insensitive
    /// </summary>
    [Theory]
    [InlineData("product")]
    [InlineData("PRODUCT")]
    [InlineData("Product")]
    public async Task SearchProductsAsync_CaseInsensitive_ReturnsMatches(string searchTerm)
    {
        // Arrange
        var product = new ProductTestDataBuilder()
            .WithName("Test Product")
            .Build();
        _mockValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult { IsValid = true });
        await _productService.CreateProductAsync(product);

        // Act
        var result = await _productService.SearchProductsAsync(searchTerm);

        // Assert
        result.Should().NotBeEmpty();
    }

    #endregion

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
