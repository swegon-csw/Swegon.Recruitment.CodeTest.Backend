using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.TestData;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Fixtures;

/// <summary>
/// Test data fixture with sample data
/// </summary>
public class TestDataFixture : IDisposable
{
    public List<Product> SampleProducts { get; private set; }
    public List<Calculation> SampleCalculations { get; private set; }
    public Dictionary<Guid, Product> ProductsById { get; private set; }
    public Dictionary<Guid, Calculation> CalculationsById { get; private set; }

    public TestDataFixture()
    {
        InitializeSampleData();
    }

    /// <summary>
    /// Initializes sample test data
    /// </summary>
    private void InitializeSampleData()
    {
        // Create sample products
        SampleProducts = new List<Product>
        {
            new ProductTestDataBuilder()
                .WithId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                .WithName("Standard Air Handler")
                .WithType(ProductType.Standard)
                .WithPrice(299.99m)
                .WithSku("SAH-001")
                .WithCategory("HVAC")
                .Build(),
                
            new ProductTestDataBuilder()
                .WithId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                .WithName("Premium Heat Pump")
                .AsPremium()
                .WithSku("PHP-002")
                .WithCategory("Heating")
                .Build(),
                
            new ProductTestDataBuilder()
                .WithId(Guid.Parse("33333333-3333-3333-3333-333333333333"))
                .WithName("Industrial Chiller")
                .AsIndustrial()
                .WithSku("ICH-003")
                .WithCategory("Cooling")
                .Build(),
                
            new ProductTestDataBuilder()
                .WithId(Guid.Parse("44444444-4444-4444-4444-444444444444"))
                .WithName("Custom Ventilation Unit")
                .AsCustom()
                .WithSku("CVU-004")
                .WithCategory("Ventilation")
                .Build(),
                
            new ProductTestDataBuilder()
                .WithId(Guid.Parse("55555555-5555-5555-5555-555555555555"))
                .WithName("Inactive Product")
                .AsInactive()
                .WithPrice(150.00m)
                .Build()
        };

        // Create sample calculations
        SampleCalculations = new List<Calculation>
        {
            new CalculationTestDataBuilder()
                .WithId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"))
                .WithProductId(SampleProducts[0].Id)
                .WithTotal(299.99m)
                .WithSubtotal(279.99m)
                .WithDiscountAmount(20.00m)
                .WithTaxAmount(59.99m)
                .AsCompleted()
                .Build(),
                
            new CalculationTestDataBuilder()
                .WithId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"))
                .WithProductId(SampleProducts[1].Id)
                .WithTotal(550.00m)
                .WithSubtotal(500.00m)
                .WithDiscountAmount(50.00m)
                .WithTaxAmount(110.00m)
                .AsCompleted()
                .Build(),
                
            new CalculationTestDataBuilder()
                .WithId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"))
                .WithProductId(SampleProducts[2].Id)
                .AsPending()
                .Build()
        };

        // Create lookup dictionaries
        ProductsById = SampleProducts.ToDictionary(p => p.Id, p => p);
        CalculationsById = SampleCalculations.ToDictionary(c => c.Id, c => c);
    }

    /// <summary>
    /// Gets a product by ID
    /// </summary>
    public Product? GetProductById(Guid id)
    {
        return ProductsById.TryGetValue(id, out var product) ? product : null;
    }

    /// <summary>
    /// Gets a calculation by ID
    /// </summary>
    public Calculation? GetCalculationById(Guid id)
    {
        return CalculationsById.TryGetValue(id, out var calculation) ? calculation : null;
    }

    /// <summary>
    /// Gets products by type
    /// </summary>
    public List<Product> GetProductsByType(ProductType type)
    {
        return SampleProducts.Where(p => p.Type == type).ToList();
    }

    /// <summary>
    /// Gets active products
    /// </summary>
    public List<Product> GetActiveProducts()
    {
        return SampleProducts.Where(p => p.IsActive).ToList();
    }

    /// <summary>
    /// Gets calculations by product
    /// </summary>
    public List<Calculation> GetCalculationsByProduct(Guid productId)
    {
        return SampleCalculations.Where(c => c.ProductId == productId).ToList();
    }

    /// <summary>
    /// Gets completed calculations
    /// </summary>
    public List<Calculation> GetCompletedCalculations()
    {
        return SampleCalculations.Where(c => c.Status == CalculationStatus.Completed).ToList();
    }

    /// <summary>
    /// Creates a new product and adds to collection
    /// </summary>
    public Product CreateAndAddProduct(ProductType type = ProductType.Standard)
    {
        var product = new ProductTestDataBuilder()
            .WithType(type)
            .Build();
        
        SampleProducts.Add(product);
        ProductsById[product.Id] = product;
        
        return product;
    }

    /// <summary>
    /// Creates a new calculation and adds to collection
    /// </summary>
    public Calculation CreateAndAddCalculation(Guid productId)
    {
        var calculation = new CalculationTestDataBuilder()
            .WithProductId(productId)
            .Build();
        
        SampleCalculations.Add(calculation);
        CalculationsById[calculation.Id] = calculation;
        
        return calculation;
    }

    /// <summary>
    /// Resets all test data to initial state
    /// </summary>
    public void Reset()
    {
        SampleProducts.Clear();
        SampleCalculations.Clear();
        ProductsById.Clear();
        CalculationsById.Clear();
        InitializeSampleData();
    }

    /// <summary>
    /// Disposes the fixture
    /// </summary>
    public void Dispose()
    {
        SampleProducts.Clear();
        SampleCalculations.Clear();
        ProductsById.Clear();
        CalculationsById.Clear();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Fixture collection for sharing TestDataFixture across tests
/// </summary>
[CollectionDefinition("Test Data")]
public class TestDataCollection : ICollectionFixture<TestDataFixture>
{
}
