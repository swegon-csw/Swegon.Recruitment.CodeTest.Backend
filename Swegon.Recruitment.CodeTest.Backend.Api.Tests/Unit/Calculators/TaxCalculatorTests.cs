using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Swegon.Recruitment.CodeTest.Backend.Api.Calculators;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.TestData;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.Utilities;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Unit.Calculators;

/// <summary>
/// Unit tests for TaxCalculator
/// </summary>
public class TaxCalculatorTests
{
    private readonly Mock<ILogger<TaxCalculator>> _mockLogger;
    private readonly TaxCalculator _calculator;

    public TaxCalculatorTests()
    {
        _mockLogger = MockHelper.CreateMockLogger<TaxCalculator>();
        _calculator = new TaxCalculator(_mockLogger.Object);
    }

    [Fact]
    public async Task CalculateAsync_WithTaxParameter_AppliesTax()
    {
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var parameters = new Dictionary<string, object> { { "taxPercentage", 0.25m } };
        var result = await _calculator.CalculateAsync(product, 1, parameters);
        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0.10)]
    [InlineData(0.20)]
    [InlineData(0.25)]
    public async Task CalculateAsync_DifferentTaxRates_CalculatesCorrectly(decimal taxRate)
    {
        var product = new ProductTestDataBuilder().WithPrice(100m).Build();
        var parameters = new Dictionary<string, object> { { "taxPercentage", taxRate } };
        var result = await _calculator.CalculateAsync(product, 1, parameters);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CalculateAsync_NoTaxParameter_NoTaxApplied()
    {
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var result = await _calculator.CalculateAsync(product, 1, new Dictionary<string, object>());
        result.TaxAmount.Should().Be(0);
    }

    [Fact]
    public async Task CalculateAsync_ZeroTax_NoTaxApplied()
    {
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var parameters = new Dictionary<string, object> { { "taxPercentage", 0m } };
        var result = await _calculator.CalculateAsync(product, 1, parameters);
        result.TaxAmount.Should().Be(0);
    }

    [Fact]
    public void CalculatorName_ReturnsCorrectName()
    {
        _calculator.CalculatorName.Should().Be("Tax Calculator");
    }
}
