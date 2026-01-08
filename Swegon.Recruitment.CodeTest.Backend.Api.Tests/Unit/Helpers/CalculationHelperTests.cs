using FluentAssertions;
using Swegon.Recruitment.CodeTest.Backend.Api.Helpers;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Unit.Helpers;

/// <summary>
/// Unit tests for CalculationHelper
/// </summary>
public class CalculationHelperTests
{
    [Theory]
    [InlineData(1, 100, 0)]
    [InlineData(10, 100, 5)]
    [InlineData(50, 100, 10)]
    public void ApplyProgressiveDiscount_VariousQuantities_ReturnsCorrectDiscount(int quantity, decimal price, decimal expectedMinDiscount)
    {
        var discount = CalculationHelper.ApplyProgressiveDiscount(quantity, price);
        discount.Should().BeGreaterOrEqualTo(expectedMinDiscount);
    }

    [Theory]
    [InlineData(100, 0.25, 25)]
    [InlineData(200, 0.10, 20)]
    public void CalculateTax_VariousAmounts_ReturnsCorrectTax(decimal amount, decimal rate, decimal expected)
    {
        var tax = CalculationHelper.CalculateTax(amount, rate);
        tax.Should().Be(expected);
    }

    [Theory]
    [InlineData(100, 10, 90)]
    [InlineData(200, 50, 150)]
    public void ApplyDiscount_VariousAmounts_ReturnsCorrectTotal(decimal amount, decimal discount, decimal expected)
    {
        var total = CalculationHelper.ApplyDiscount(amount, discount);
        total.Should().Be(expected);
    }

    [Fact]
    public void CalculateSubtotal_ValidInputs_ReturnsCorrectSubtotal()
    {
        var subtotal = CalculationHelper.CalculateSubtotal(100m, 5);
        subtotal.Should().Be(500m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CalculateSubtotal_InvalidQuantity_ThrowsException(int quantity)
    {
        Assert.Throws<ArgumentException>(() => 
            CalculationHelper.CalculateSubtotal(100m, quantity));
    }

    [Fact]
    public void RoundToTwoDecimals_VariousValues_RoundsCorrectly()
    {
        var result = CalculationHelper.RoundToTwoDecimals(10.12345m);
        result.Should().Be(10.12m);
    }
}
