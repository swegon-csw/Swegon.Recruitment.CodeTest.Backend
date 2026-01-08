using FluentAssertions;
using Swegon.Recruitment.CodeTest.Backend.Api.Helpers;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.TestData;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Unit.Helpers;

/// <summary>
/// Unit tests for ValidationHelper
/// </summary>
public class ValidationHelperTests
{
    [Fact]
    public void ValidateProduct_ValidProduct_ReturnsTrue()
    {
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var result = ValidationHelper.IsValidProduct(product);
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateProduct_NullProduct_ReturnsFalse()
    {
        var result = ValidationHelper.IsValidProduct(null);
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidatePrice_ValidPrice_ReturnsTrue()
    {
        var result = ValidationHelper.IsValidPrice(100m);
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void ValidatePrice_NegativePrice_ReturnsFalse(decimal price)
    {
        var result = ValidationHelper.IsValidPrice(price);
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("ABC")]
    [InlineData("Valid Name")]
    [InlineData("Product 123")]
    public void ValidateName_ValidName_ReturnsTrue(string name)
    {
        var result = ValidationHelper.IsValidName(name);
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("AB")]
    public void ValidateName_InvalidName_ReturnsFalse(string? name)
    {
        var result = ValidationHelper.IsValidName(name);
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(1000)]
    public void ValidateQuantity_ValidQuantity_ReturnsTrue(int quantity)
    {
        var result = ValidationHelper.IsValidQuantity(quantity);
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ValidateQuantity_InvalidQuantity_ReturnsFalse(int quantity)
    {
        var result = ValidationHelper.IsValidQuantity(quantity);
        result.Should().BeFalse();
    }
}
