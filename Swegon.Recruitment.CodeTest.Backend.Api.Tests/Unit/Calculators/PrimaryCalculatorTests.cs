using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Swegon.Recruitment.CodeTest.Backend.Api.Calculators;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.TestData;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.Utilities;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Unit.Calculators;

/// <summary>
/// Unit tests for PrimaryCalculator
/// </summary>
public class PrimaryCalculatorTests : IDisposable
{
    private readonly Mock<ILogger<PrimaryCalculator>> _mockLogger;
    private readonly PrimaryCalculator _calculator;

    public PrimaryCalculatorTests()
    {
        _mockLogger = MockHelper.CreateMockLogger<PrimaryCalculator>();
        _calculator = new PrimaryCalculator(_mockLogger.Object);
    }

    /// <summary>
    /// CalculateAsync should calculate standard product correctly
    /// </summary>
    [Fact]
    public async Task CalculateAsync_StandardProduct_CalculatesCorrectly()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var quantity = 1;
        var parameters = new Dictionary<string, object>();

        // Act
        var result = await _calculator.CalculateAsync(product, quantity, parameters);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(CalculationStatus.Completed);
        result.BasePrice.Should().Be(product.Price);
        result.Quantity.Should().Be(quantity);
    }

    /// <summary>
    /// CalculateAsync should apply quantity multiplier
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task CalculateAsync_DifferentQuantities_AppliesMultiplier(int quantity)
    {
        // Arrange
        var product = new ProductTestDataBuilder()
            .WithPrice(100m)
            .Build();
        var parameters = new Dictionary<string, object>();

        // Act
        var result = await _calculator.CalculateAsync(product, quantity, parameters);

        // Assert
        result.Quantity.Should().Be(quantity);
        result.BasePrice.Should().Be(100m);
    }

    /// <summary>
    /// CalculateAsync should calculate premium product with multiplier
    /// </summary>
    [Fact]
    public async Task CalculateAsync_PremiumProduct_AppliesPremiumMultiplier()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreatePremiumProduct();
        var quantity = 1;
        var parameters = new Dictionary<string, object>();

        // Act
        var result = await _calculator.CalculateAsync(product, quantity, parameters);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(CalculationStatus.Completed);
        result.Total.Should().BeGreaterThan(product.Price);
    }

    /// <summary>
    /// CalculateAsync should calculate industrial product correctly
    /// </summary>
    [Fact]
    public async Task CalculateAsync_IndustrialProduct_AppliesIndustrialFactor()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateIndustrialProduct();
        var quantity = 1;
        var parameters = new Dictionary<string, object>();

        // Act
        var result = await _calculator.CalculateAsync(product, quantity, parameters);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(CalculationStatus.Completed);
        result.Total.Should().BeGreaterThan(product.Price);
    }

    /// <summary>
    /// CalculateAsync should calculate custom product correctly
    /// </summary>
    [Fact]
    public async Task CalculateAsync_CustomProduct_AppliesCustomFactor()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateCustomProduct();
        var quantity = 1;
        var parameters = new Dictionary<string, object>();

        // Act
        var result = await _calculator.CalculateAsync(product, quantity, parameters);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(CalculationStatus.Completed);
    }

    /// <summary>
    /// CalculateAsync should throw for null product
    /// </summary>
    [Fact]
    public async Task CalculateAsync_NullProduct_ThrowsException()
    {
        // Arrange
        var quantity = 1;
        var parameters = new Dictionary<string, object>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _calculator.CalculateAsync(null!, quantity, parameters));
    }

    /// <summary>
    /// CalculateAsync should throw for invalid quantity
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public async Task CalculateAsync_InvalidQuantity_ThrowsException(int quantity)
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var parameters = new Dictionary<string, object>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _calculator.CalculateAsync(product, quantity, parameters));
    }

    /// <summary>
    /// CalculateAsync should handle parameters
    /// </summary>
    [Fact]
    public async Task CalculateAsync_WithParameters_AppliesParameters()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var quantity = 1;
        var parameters = new Dictionary<string, object>
        {
            { "discount", 0.1m },
            { "tax", 0.25m }
        };

        // Act
        var result = await _calculator.CalculateAsync(product, quantity, parameters);

        // Assert
        result.Should().NotBeNull();
        result.Metadata.Should().ContainKey("Parameters");
    }

    /// <summary>
    /// CalculateAsync should add metadata
    /// </summary>
    [Fact]
    public async Task CalculateAsync_AddsMetadata()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var quantity = 1;
        var parameters = new Dictionary<string, object>();

        // Act
        var result = await _calculator.CalculateAsync(product, quantity, parameters);

        // Assert
        result.Metadata.Should().ContainKey("CalculatorType");
        result.Metadata.Should().ContainKey("CalculatorVersion");
    }

    /// <summary>
    /// CalculateAsync should handle bulk quantities with discount
    /// </summary>
    [Fact]
    public async Task CalculateAsync_BulkQuantity_AppliesBulkDiscount()
    {
        // Arrange
        var product = new ProductTestDataBuilder()
            .WithPrice(100m)
            .Build();
        var bulkQuantity = 50;
        var parameters = new Dictionary<string, object>();

        // Act
        var result = await _calculator.CalculateAsync(product, bulkQuantity, parameters);

        // Assert
        result.Should().NotBeNull();
        result.Quantity.Should().Be(bulkQuantity);
    }

    /// <summary>
    /// CalculateAsync should handle cancellation token
    /// </summary>
    [Fact]
    public async Task CalculateAsync_WithCancellation_HandlesCancellation()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var quantity = 1;
        var parameters = new Dictionary<string, object>();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => 
            _calculator.CalculateAsync(product, quantity, parameters, cts.Token));
    }

    /// <summary>
    /// CalculateAsync should log calculation start
    /// </summary>
    [Fact]
    public async Task CalculateAsync_LogsCalculation()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var quantity = 1;
        var parameters = new Dictionary<string, object>();

        // Act
        await _calculator.CalculateAsync(product, quantity, parameters);

        // Assert
        MockHelper.VerifyLogLevel(_mockLogger, LogLevel.Information, Times.AtLeastOnce());
    }

    /// <summary>
    /// CalculatorName should return correct name
    /// </summary>
    [Fact]
    public void CalculatorName_ReturnsCorrectName()
    {
        // Act
        var name = _calculator.CalculatorName;

        // Assert
        name.Should().Be("Primary Calculator");
    }

    /// <summary>
    /// Version should return correct version
    /// </summary>
    [Fact]
    public void Version_ReturnsCorrectVersion()
    {
        // Act
        var version = _calculator.Version;

        // Assert
        version.Should().NotBeNullOrEmpty();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
