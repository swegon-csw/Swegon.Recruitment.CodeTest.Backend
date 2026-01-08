using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Swegon.Recruitment.CodeTest.Backend.Api.Services;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.TestData;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.Utilities;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Unit.Services;

/// <summary>
/// Unit tests for CacheService
/// </summary>
public class CacheServiceTests : IDisposable
{
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<ILogger<CacheService>> _mockLogger;
    private readonly CacheService _cacheService;

    public CacheServiceTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _mockLogger = MockHelper.CreateMockLogger<CacheService>();
        _cacheService = new CacheService(_memoryCache, _mockLogger.Object);
    }

    /// <summary>
    /// GetAsync should return null for non-existent key
    /// </summary>
    [Fact]
    public async Task GetAsync_NonExistentKey_ReturnsNull()
    {
        // Arrange
        var key = "non-existent-key";

        // Act
        var result = await _cacheService.GetAsync<Product>(key);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// GetAsync should throw for null or empty key
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetAsync_InvalidKey_ThrowsException(string? key)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _cacheService.GetAsync<Product>(key!));
    }

    /// <summary>
    /// SetAsync should cache value successfully
    /// </summary>
    [Fact]
    public async Task SetAsync_ValidValue_CachesSuccessfully()
    {
        // Arrange
        var key = "test-key";
        var product = ProductTestDataBuilder.CreateStandardProduct();

        // Act
        await _cacheService.SetAsync(key, product);
        var result = await _cacheService.GetAsync<Product>(key);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
    }

    /// <summary>
    /// SetAsync should throw for null key
    /// </summary>
    [Fact]
    public async Task SetAsync_NullKey_ThrowsException()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _cacheService.SetAsync(null!, product));
    }

    /// <summary>
    /// SetAsync should throw for null value
    /// </summary>
    [Fact]
    public async Task SetAsync_NullValue_ThrowsException()
    {
        // Arrange
        var key = "test-key";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _cacheService.SetAsync<Product>(key, null!));
    }

    /// <summary>
    /// SetAsync should respect expiration time
    /// </summary>
    [Fact]
    public async Task SetAsync_WithExpiration_ExpiresAfterTime()
    {
        // Arrange
        var key = "expiring-key";
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var expiration = TimeSpan.FromMilliseconds(100);

        // Act
        await _cacheService.SetAsync(key, product, expiration);
        var immediate = await _cacheService.GetAsync<Product>(key);
        
        await Task.Delay(150);
        var afterExpiry = await _cacheService.GetAsync<Product>(key);

        // Assert
        immediate.Should().NotBeNull();
        afterExpiry.Should().BeNull();
    }

    /// <summary>
    /// RemoveAsync should remove cached value
    /// </summary>
    [Fact]
    public async Task RemoveAsync_ExistingKey_RemovesValue()
    {
        // Arrange
        var key = "test-key";
        var product = ProductTestDataBuilder.CreateStandardProduct();
        await _cacheService.SetAsync(key, product);

        // Act
        await _cacheService.RemoveAsync(key);
        var result = await _cacheService.GetAsync<Product>(key);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// RemoveAsync should not throw for non-existent key
    /// </summary>
    [Fact]
    public async Task RemoveAsync_NonExistentKey_DoesNotThrow()
    {
        // Arrange
        var key = "non-existent-key";

        // Act
        var action = async () => await _cacheService.RemoveAsync(key);

        // Assert
        await action.Should().NotThrowAsync();
    }

    /// <summary>
    /// GetOrCreateAsync should create value if not cached
    /// </summary>
    [Fact]
    public async Task GetOrCreateAsync_NotCached_CreatesValue()
    {
        // Arrange
        var key = "new-key";
        var product = ProductTestDataBuilder.CreateStandardProduct();

        // Act
        var result = await _cacheService.GetOrCreateAsync(key, 
            async () => await Task.FromResult(product));

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
    }

    /// <summary>
    /// GetOrCreateAsync should return cached value if exists
    /// </summary>
    [Fact]
    public async Task GetOrCreateAsync_Cached_ReturnsCachedValue()
    {
        // Arrange
        var key = "cached-key";
        var cachedProduct = ProductTestDataBuilder.CreateStandardProduct();
        var newProduct = ProductTestDataBuilder.CreatePremiumProduct();
        
        await _cacheService.SetAsync(key, cachedProduct);

        // Act
        var result = await _cacheService.GetOrCreateAsync(key, 
            async () => await Task.FromResult(newProduct));

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cachedProduct.Id);
        result.Id.Should().NotBe(newProduct.Id);
    }

    /// <summary>
    /// Multiple concurrent gets should work correctly
    /// </summary>
    [Fact]
    public async Task ConcurrentGets_MultipleThreads_WorkCorrectly()
    {
        // Arrange
        var key = "concurrent-key";
        var product = ProductTestDataBuilder.CreateStandardProduct();
        await _cacheService.SetAsync(key, product);

        // Act
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => _cacheService.GetAsync<Product>(key))
            .ToArray();
        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().AllSatisfy(r => 
        {
            r.Should().NotBeNull();
            r!.Id.Should().Be(product.Id);
        });
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
        GC.SuppressFinalize(this);
    }
}
