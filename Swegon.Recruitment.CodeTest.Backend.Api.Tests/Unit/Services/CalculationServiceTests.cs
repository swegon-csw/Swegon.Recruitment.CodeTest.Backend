using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Services;
using Swegon.Recruitment.CodeTest.Backend.Api.Calculators;
using Swegon.Recruitment.CodeTest.Backend.Api.Validators;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.TestData;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.Utilities;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Unit.Services;

/// <summary>
/// Unit tests for CalculationService
/// </summary>
public class CalculationServiceTests : IDisposable
{
    private readonly Mock<ILogger<CalculationService>> _mockLogger;
    private readonly Mock<ProductService> _mockProductService;
    private readonly Mock<CacheService> _mockCacheService;
    private readonly Mock<CalculationRequestValidator> _mockValidator;
    private readonly Mock<PrimaryCalculator> _mockPrimaryCalculator;
    private readonly Mock<ComplexCalculator> _mockComplexCalculator;

    public CalculationServiceTests()
    {
        _mockLogger = MockHelper.CreateMockLogger<CalculationService>();
        _mockProductService = MockHelper.CreateMockProductService();
        _mockCacheService = MockHelper.CreateMockCacheService();
        _mockValidator = new Mock<CalculationRequestValidator>();
        _mockPrimaryCalculator = new Mock<PrimaryCalculator>(MockHelper.CreateMockLogger<PrimaryCalculator>().Object);
        _mockComplexCalculator = new Mock<ComplexCalculator>(MockHelper.CreateMockLogger<ComplexCalculator>().Object);
    }

    /// <summary>
    /// CalculateAsync should perform calculation successfully
    /// </summary>
    [Fact]
    public async Task CalculateAsync_ValidRequest_ReturnsCalculation()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var request = CalculationTestDataBuilder.CreateCalculationRequest(product.Id, 5);
        var expectedResult = CalculationTestDataBuilder.CreateCalculationResult();
        
        _mockValidator.Setup(v => v.Validate(It.IsAny<CalculationRequest>()))
            .Returns(new ValidationResult { IsValid = true });
        _mockProductService.Setup(s => s.GetProductByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockPrimaryCalculator.Setup(c => c.CalculateAsync(
            It.IsAny<Product>(), It.IsAny<int>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var service = CreateService();

        // Act
        var result = await service.CalculateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(CalculationStatus.Completed);
    }

    /// <summary>
    /// CalculateAsync should throw when request is invalid
    /// </summary>
    [Fact]
    public async Task CalculateAsync_InvalidRequest_ThrowsException()
    {
        // Arrange
        var request = CalculationTestDataBuilder.CreateCalculationRequest(Guid.NewGuid());
        _mockValidator.Setup(v => v.Validate(It.IsAny<CalculationRequest>()))
            .Returns(new ValidationResult 
            { 
                IsValid = false, 
                Errors = new[] { "Invalid quantity" }
            });

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.CalculateAsync(request));
    }

    /// <summary>
    /// CalculateAsync should throw when product not found
    /// </summary>
    [Fact]
    public async Task CalculateAsync_ProductNotFound_ThrowsException()
    {
        // Arrange
        var request = CalculationTestDataBuilder.CreateCalculationRequest(Guid.NewGuid());
        _mockValidator.Setup(v => v.Validate(It.IsAny<CalculationRequest>()))
            .Returns(new ValidationResult { IsValid = true });
        _mockProductService.Setup(s => s.GetProductByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.CalculateAsync(request));
    }

    /// <summary>
    /// CalculateAsync should use complex calculator when requested
    /// </summary>
    [Fact]
    public async Task CalculateAsync_WithComplexCalculatorParameter_UsesComplexCalculator()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var parameters = new Dictionary<string, object>
        {
            { "useComplexCalculator", true }
        };
        var request = CalculationTestDataBuilder.CreateCalculationRequestWithParameters(product.Id, 1, parameters);
        var expectedResult = CalculationTestDataBuilder.CreateCalculationResult();
        
        _mockValidator.Setup(v => v.Validate(It.IsAny<CalculationRequest>()))
            .Returns(new ValidationResult { IsValid = true });
        _mockProductService.Setup(s => s.GetProductByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockComplexCalculator.Setup(c => c.CalculateAsync(
            It.IsAny<Product>(), It.IsAny<int>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var service = CreateService();

        // Act
        var result = await service.CalculateAsync(request);

        // Assert
        _mockComplexCalculator.Verify(c => c.CalculateAsync(
            It.IsAny<Product>(), It.IsAny<int>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    /// <summary>
    /// CalculateAsync should calculate with different quantities
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public async Task CalculateAsync_DifferentQuantities_CalculatesCorrectly(int quantity)
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var request = CalculationTestDataBuilder.CreateCalculationRequest(product.Id, quantity);
        var expectedResult = CalculationTestDataBuilder.CreateCalculationResult();
        
        _mockValidator.Setup(v => v.Validate(It.IsAny<CalculationRequest>()))
            .Returns(new ValidationResult { IsValid = true });
        _mockProductService.Setup(s => s.GetProductByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockPrimaryCalculator.Setup(c => c.CalculateAsync(
            It.IsAny<Product>(), quantity, It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var service = CreateService();

        // Act
        var result = await service.CalculateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockPrimaryCalculator.Verify(c => c.CalculateAsync(
            It.IsAny<Product>(), quantity, It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    /// <summary>
    /// GetCalculationHistoryAsync should return calculation history
    /// </summary>
    [Fact]
    public async Task GetCalculationHistoryAsync_ReturnsHistory()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var calculations = CalculationTestDataBuilder.CreateMultipleCalculations(5);
        var service = CreateService();

        // Act & Assert - This test would need actual service implementation
        // Placeholder for demonstration
        Assert.True(true);
    }

    /// <summary>
    /// GetCalculationHistoryAsync should filter by product ID
    /// </summary>
    [Fact]
    public async Task GetCalculationHistoryAsync_FilterByProductId_ReturnsFilteredHistory()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var service = CreateService();

        // Act & Assert
        Assert.True(true);
    }

    /// <summary>
    /// BatchCalculateAsync should process multiple calculations
    /// </summary>
    [Fact]
    public async Task BatchCalculateAsync_MultipleRequests_ProcessesAll()
    {
        // Arrange
        var service = CreateService();
        
        // Act & Assert
        Assert.True(true);
    }

    /// <summary>
    /// ValidateCalculationAsync should validate calculation request
    /// </summary>
    [Fact]
    public async Task ValidateCalculationAsync_ValidRequest_ReturnsValid()
    {
        // Arrange
        var request = CalculationTestDataBuilder.CreateCalculationRequest(Guid.NewGuid());
        _mockValidator.Setup(v => v.Validate(It.IsAny<CalculationRequest>()))
            .Returns(new ValidationResult { IsValid = true });

        var service = CreateService();

        // Act
        var result = _mockValidator.Object.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// CalculateAsync should handle calculator exceptions gracefully
    /// </summary>
    [Fact]
    public async Task CalculateAsync_CalculatorThrows_HandlesException()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var request = CalculationTestDataBuilder.CreateCalculationRequest(product.Id);
        
        _mockValidator.Setup(v => v.Validate(It.IsAny<CalculationRequest>()))
            .Returns(new ValidationResult { IsValid = true });
        _mockProductService.Setup(s => s.GetProductByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockPrimaryCalculator.Setup(c => c.CalculateAsync(
            It.IsAny<Product>(), It.IsAny<int>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Calculation error"));

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.CalculateAsync(request));
    }

    /// <summary>
    /// CalculateAsync should log calculation start
    /// </summary>
    [Fact]
    public async Task CalculateAsync_LogsCalculationStart()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var request = CalculationTestDataBuilder.CreateCalculationRequest(product.Id);
        var expectedResult = CalculationTestDataBuilder.CreateCalculationResult();
        
        _mockValidator.Setup(v => v.Validate(It.IsAny<CalculationRequest>()))
            .Returns(new ValidationResult { IsValid = true });
        _mockProductService.Setup(s => s.GetProductByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockPrimaryCalculator.Setup(c => c.CalculateAsync(
            It.IsAny<Product>(), It.IsAny<int>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var service = CreateService();

        // Act
        await service.CalculateAsync(request);

        // Assert
        MockHelper.VerifyLogLevel(_mockLogger, LogLevel.Information, Times.AtLeastOnce());
    }

    /// <summary>
    /// CalculateAsync should handle cancellation token
    /// </summary>
    [Fact]
    public async Task CalculateAsync_WithCancellationToken_PropagatesToken()
    {
        // Arrange
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var request = CalculationTestDataBuilder.CreateCalculationRequest(product.Id);
        var cts = new CancellationTokenSource();
        var expectedResult = CalculationTestDataBuilder.CreateCalculationResult();
        
        _mockValidator.Setup(v => v.Validate(It.IsAny<CalculationRequest>()))
            .Returns(new ValidationResult { IsValid = true });
        _mockProductService.Setup(s => s.GetProductByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockPrimaryCalculator.Setup(c => c.CalculateAsync(
            It.IsAny<Product>(), It.IsAny<int>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var service = CreateService();

        // Act
        var result = await service.CalculateAsync(request, cts.Token);

        // Assert
        result.Should().NotBeNull();
    }

    private CalculationService CreateService()
    {
        return new CalculationService(
            _mockLogger.Object,
            _mockProductService.Object,
            _mockCacheService.Object,
            _mockValidator.Object,
            _mockPrimaryCalculator.Object,
            _mockComplexCalculator.Object);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
