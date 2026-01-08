using Microsoft.Extensions.Logging;
using Moq;
using Swegon.Recruitment.CodeTest.Backend.Api.Services;
using Swegon.Recruitment.CodeTest.Backend.Api.Validators;
using Swegon.Recruitment.CodeTest.Backend.Api.Calculators;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Fixtures;

/// <summary>
/// Service fixture for unit tests with mock dependencies
/// </summary>
public class ServiceFixture : IDisposable
{
    public Mock<ILogger<ProductService>> MockProductLogger { get; }
    public Mock<ILogger<CalculationService>> MockCalculationLogger { get; }
    public Mock<ILogger<CacheService>> MockCacheLogger { get; }
    public Mock<ILogger<ValidationService>> MockValidationLogger { get; }
    public Mock<ILogger<ExportService>> MockExportLogger { get; }
    public Mock<ILogger<PrimaryCalculator>> MockPrimaryCalculatorLogger { get; }
    
    public Mock<ProductValidator> MockProductValidator { get; }
    public Mock<CalculationRequestValidator> MockCalculationValidator { get; }
    
    public Mock<CacheService> MockCacheService { get; }
    public Mock<ProductService> MockProductService { get; }
    public Mock<ValidationService> MockValidationService { get; }

    public ServiceFixture()
    {
        // Initialize loggers
        MockProductLogger = new Mock<ILogger<ProductService>>();
        MockCalculationLogger = new Mock<ILogger<CalculationService>>();
        MockCacheLogger = new Mock<ILogger<CacheService>>();
        MockValidationLogger = new Mock<ILogger<ValidationService>>();
        MockExportLogger = new Mock<ILogger<ExportService>>();
        MockPrimaryCalculatorLogger = new Mock<ILogger<PrimaryCalculator>>();
        
        // Initialize validators
        MockProductValidator = new Mock<ProductValidator>();
        MockCalculationValidator = new Mock<CalculationRequestValidator>();
        
        // Initialize services
        MockCacheService = new Mock<CacheService>(MockCacheLogger.Object);
        MockProductService = new Mock<ProductService>(
            MockProductValidator.Object,
            MockCacheService.Object,
            MockProductLogger.Object);
        MockValidationService = new Mock<ValidationService>(MockValidationLogger.Object);
    }

    /// <summary>
    /// Creates a ProductService with mocked dependencies
    /// </summary>
    public ProductService CreateProductService()
    {
        return new ProductService(
            MockProductValidator.Object,
            MockCacheService.Object,
            MockProductLogger.Object);
    }

    /// <summary>
    /// Creates a CacheService
    /// </summary>
    public CacheService CreateCacheService()
    {
        return new CacheService(MockCacheLogger.Object);
    }

    /// <summary>
    /// Creates a ValidationService
    /// </summary>
    public ValidationService CreateValidationService()
    {
        return new ValidationService(MockValidationLogger.Object);
    }

    /// <summary>
    /// Creates an ExportService with mocked dependencies
    /// </summary>
    public ExportService CreateExportService()
    {
        return new ExportService(MockExportLogger.Object);
    }

    /// <summary>
    /// Creates a PrimaryCalculator
    /// </summary>
    public PrimaryCalculator CreatePrimaryCalculator()
    {
        return new PrimaryCalculator(MockPrimaryCalculatorLogger.Object);
    }

    /// <summary>
    /// Resets all mocks
    /// </summary>
    public void ResetMocks()
    {
        MockProductLogger.Reset();
        MockCalculationLogger.Reset();
        MockCacheLogger.Reset();
        MockValidationLogger.Reset();
        MockExportLogger.Reset();
        MockPrimaryCalculatorLogger.Reset();
        MockProductValidator.Reset();
        MockCalculationValidator.Reset();
        MockCacheService.Reset();
        MockProductService.Reset();
        MockValidationService.Reset();
    }

    /// <summary>
    /// Disposes the fixture
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Fixture collection for sharing ServiceFixture across tests
/// </summary>
[CollectionDefinition("Service Tests")]
public class ServiceTestCollection : ICollectionFixture<ServiceFixture>
{
}
