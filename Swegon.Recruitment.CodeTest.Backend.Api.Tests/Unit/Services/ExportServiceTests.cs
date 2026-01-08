using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Swegon.Recruitment.CodeTest.Backend.Api.Services;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.TestData;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.Utilities;
using System.Text.Json;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Unit.Services;

/// <summary>
/// Unit tests for ExportService
/// </summary>
public class ExportServiceTests : IDisposable
{
    private readonly Mock<ILogger<ExportService>> _mockLogger;
    private readonly ExportService _exportService;

    public ExportServiceTests()
    {
        _mockLogger = MockHelper.CreateMockLogger<ExportService>();
        _exportService = new ExportService(_mockLogger.Object);
    }

    /// <summary>
    /// ExportToCsvAsync should export products to CSV
    /// </summary>
    [Fact]
    public async Task ExportToCsvAsync_Products_ReturnsCsvString()
    {
        // Arrange
        var products = ProductTestDataBuilder.CreateMultipleProducts(3);

        // Act
        var result = await _exportService.ExportToCsvAsync(products);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Id");
        result.Should().Contain("Name");
        result.Should().Contain("Price");
    }

    /// <summary>
    /// ExportToCsvAsync should handle empty collection
    /// </summary>
    [Fact]
    public async Task ExportToCsvAsync_EmptyCollection_ReturnsHeaderOnly()
    {
        // Arrange
        var products = new List<Product>();

        // Act
        var result = await _exportService.ExportToCsvAsync(products);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Id");
        result.Split('\n').Should().HaveCount(2); // Header + empty line
    }

    /// <summary>
    /// ExportToCsvAsync should export calculations to CSV
    /// </summary>
    [Fact]
    public async Task ExportToCsvAsync_Calculations_ReturnsCsvString()
    {
        // Arrange
        var calculations = CalculationTestDataBuilder.CreateMultipleCalculations(3);

        // Act
        var result = await _exportService.ExportToCsvAsync(calculations);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("ProductId");
        result.Should().Contain("Total");
    }

    /// <summary>
    /// ExportToJsonAsync should export products to JSON
    /// </summary>
    [Fact]
    public async Task ExportToJsonAsync_Products_ReturnsJsonString()
    {
        // Arrange
        var products = ProductTestDataBuilder.CreateMultipleProducts(3);

        // Act
        var result = await _exportService.ExportToJsonAsync(products);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("\"Id\"");
        result.Should().Contain("\"Name\"");
        
        // Verify it's valid JSON
        var deserialized = JsonSerializer.Deserialize<List<Product>>(result);
        deserialized.Should().NotBeNull();
        deserialized.Should().HaveCount(3);
    }

    /// <summary>
    /// ExportToJsonAsync should handle empty collection
    /// </summary>
    [Fact]
    public async Task ExportToJsonAsync_EmptyCollection_ReturnsEmptyArray()
    {
        // Arrange
        var products = new List<Product>();

        // Act
        var result = await _exportService.ExportToJsonAsync(products);

        // Assert
        result.Should().Be("[]");
    }

    /// <summary>
    /// ExportToJsonAsync should export calculations to JSON
    /// </summary>
    [Fact]
    public async Task ExportToJsonAsync_Calculations_ReturnsJsonString()
    {
        // Arrange
        var calculations = CalculationTestDataBuilder.CreateMultipleCalculations(5);

        // Act
        var result = await _exportService.ExportToJsonAsync(calculations);

        // Assert
        result.Should().NotBeNullOrEmpty();
        var deserialized = JsonSerializer.Deserialize<List<Calculation>>(result);
        deserialized.Should().HaveCount(5);
    }

    /// <summary>
    /// ExportToCsvAsync should log export operation
    /// </summary>
    [Fact]
    public async Task ExportToCsvAsync_LogsOperation()
    {
        // Arrange
        var products = ProductTestDataBuilder.CreateMultipleProducts(2);

        // Act
        await _exportService.ExportToCsvAsync(products);

        // Assert
        MockHelper.VerifyLogLevel(_mockLogger, LogLevel.Information, Times.Once());
    }

    /// <summary>
    /// ExportToJsonAsync should log export operation
    /// </summary>
    [Fact]
    public async Task ExportToJsonAsync_LogsOperation()
    {
        // Arrange
        var products = ProductTestDataBuilder.CreateMultipleProducts(2);

        // Act
        await _exportService.ExportToJsonAsync(products);

        // Assert
        MockHelper.VerifyLogLevel(_mockLogger, LogLevel.Information, Times.Once());
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
