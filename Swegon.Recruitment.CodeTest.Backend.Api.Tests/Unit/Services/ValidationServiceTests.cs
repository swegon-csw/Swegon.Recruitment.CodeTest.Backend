using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Swegon.Recruitment.CodeTest.Backend.Api.Services;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Validators;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.TestData;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.Utilities;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Unit.Services;

/// <summary>
/// Unit tests for ValidationService
/// </summary>
public class ValidationServiceTests : IDisposable
{
    private readonly Mock<ILogger<ValidationService>> _mockLogger;
    private readonly Mock<ProductValidator> _mockProductValidator;
    private readonly Mock<CalculationRequestValidator> _mockCalculationValidator;
    private readonly ValidationService _validationService;

    public ValidationServiceTests()
    {
        _mockLogger = MockHelper.CreateMockLogger<ValidationService>();
        _mockProductValidator = new Mock<ProductValidator>();
        _mockCalculationValidator = new Mock<CalculationRequestValidator>();
        
        _validationService = new ValidationService(
            _mockLogger.Object,
            _mockProductValidator.Object,
            _mockCalculationValidator.Object);
    }

    /// <summary>
    /// ValidateProductAsync should return valid for valid product
    /// </summary>
    [Fact]
    public async Task ValidateProductAsync_ValidProduct_ReturnsValid()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        _mockProductValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult { IsValid = true });

        // Act
        var result = await _validationService.ValidateProductAsync(product);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// ValidateProductAsync should return invalid for invalid product
    /// </summary>
    [Fact]
    public async Task ValidateProductAsync_InvalidProduct_ReturnsInvalid()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        _mockProductValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult 
            { 
                IsValid = false,
                Errors = new[] { "Name is required" }
            });

        // Act
        var result = await _validationService.ValidateProductAsync(product);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Name is required");
    }

    /// <summary>
    /// ValidateProductAsync should validate product name
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("AB")]
    [InlineData(null)]
    public async Task ValidateProductAsync_InvalidName_ReturnsInvalid(string? name)
    {
        // Arrange
        var product = new ProductTestDataBuilder()
            .WithName(name!)
            .Build();
        _mockProductValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult 
            { 
                IsValid = false,
                Errors = new[] { "Name must be between 3 and 100 characters" }
            });

        // Act
        var result = await _validationService.ValidateProductAsync(product);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    /// <summary>
    /// ValidateProductAsync should validate price range
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task ValidateProductAsync_NegativePrice_ReturnsInvalid(decimal price)
    {
        // Arrange
        var product = new ProductTestDataBuilder()
            .WithPrice(price)
            .Build();
        _mockProductValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult 
            { 
                IsValid = false,
                Errors = new[] { "Price must be greater than or equal to 0" }
            });

        // Act
        var result = await _validationService.ValidateProductAsync(product);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    /// <summary>
    /// ValidateAsync should validate generic entities
    /// </summary>
    [Fact]
    public async Task ValidateAsync_GenericEntity_ReturnsValid()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();

        // Act
        var result = await _validationService.ValidateAsync(product);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// ValidateProductAsync should log validation
    /// </summary>
    [Fact]
    public async Task ValidateProductAsync_LogsValidation()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        _mockProductValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult { IsValid = true });

        // Act
        await _validationService.ValidateProductAsync(product);

        // Assert
        MockHelper.VerifyLogLevel(_mockLogger, LogLevel.Information, Times.AtLeastOnce());
    }

    /// <summary>
    /// ValidateProductAsync should handle validator exceptions
    /// </summary>
    [Fact]
    public async Task ValidateProductAsync_ValidatorThrows_PropagatesException()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        _mockProductValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Throws(new InvalidOperationException("Validator error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _validationService.ValidateProductAsync(product));
    }

    /// <summary>
    /// ValidateProductAsync should validate multiple products
    /// </summary>
    [Fact]
    public async Task ValidateProductAsync_MultipleProducts_ValidatesEach()
    {
        // Arrange
        var products = ProductTestDataBuilder.CreateMultipleProducts(5);
        _mockProductValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult { IsValid = true });

        // Act
        var tasks = products.Select(p => _validationService.ValidateProductAsync(p));
        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().AllSatisfy(r => r.IsValid.Should().BeTrue());
    }

    /// <summary>
    /// ValidateProductAsync should validate product type
    /// </summary>
    [Fact]
    public async Task ValidateProductAsync_ValidatesProductType()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreatePremiumProduct();
        _mockProductValidator.Setup(v => v.Validate(It.IsAny<Product>()))
            .Returns(new ValidationResult { IsValid = true });

        // Act
        var result = await _validationService.ValidateProductAsync(product);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// ValidateAsync should work with different entity types
    /// </summary>
    [Fact]
    public async Task ValidateAsync_DifferentTypes_ValidatesCorrectly()
    {
        // Arrange
        var calculation = CalculationTestDataBuilder.CreateDefaultCalculation();

        // Act
        var result = await _validationService.ValidateAsync(calculation);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
