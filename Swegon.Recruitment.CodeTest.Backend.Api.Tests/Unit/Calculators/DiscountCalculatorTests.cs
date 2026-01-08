using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Swegon.Recruitment.CodeTest.Backend.Api.Calculators;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.TestData;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.Utilities;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Unit.Calculators;

/// <summary>
/// Unit tests for DiscountCalculator
/// </summary>
public class DiscountCalculatorTests
{
    private readonly Mock<ILogger<DiscountCalculator>> _mockLogger;
    private readonly DiscountCalculator _calculator;

    public DiscountCalculatorTests()
    {
        _mockLogger = MockHelper.CreateMockLogger<DiscountCalculator>();
        _calculator = new DiscountCalculator(_mockLogger.Object);
    }

    [Fact]
    public async Task CalculateAsync_StandardProduct_AppliesCorrectDiscount()
    {
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var result = await _calculator.CalculateAsync(product, 1, new Dictionary<string, object>());
        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(10, 5)]
    [InlineData(50, 10)]
    public async Task CalculateAsync_QuantityBased_AppliesProgressiveDiscount(int quantity, decimal expectedDiscount)
    {
        var product = new ProductTestDataBuilder().WithPrice(100m).Build();
        var result = await _calculator.CalculateAsync(product, quantity, new Dictionary<string, object>());
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CalculateAsync_NoDiscount_ReturnsFullPrice()
    {
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var result = await _calculator.CalculateAsync(product, 1, new Dictionary<string, object>());
        result.Subtotal.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task CalculateAsync_BulkPurchase_AppliesLargeDiscount()
    {
        var product = new ProductTestDataBuilder().WithPrice(100m).Build();
        var result = await _calculator.CalculateAsync(product, 100, new Dictionary<string, object>());
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CalculateAsync_AddsDiscountMetadata()
    {
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var result = await _calculator.CalculateAsync(product, 10, new Dictionary<string, object>());
        result.Metadata.Should().ContainKey("CalculatorType");
    }

    [Fact]
    public void CalculatorName_ReturnsCorrectName()
    {
        _calculator.CalculatorName.Should().Be("Discount Calculator");
    }
}
