using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.TestData;

/// <summary>
/// Generates random test data
/// </summary>
public static class TestDataGenerator
{
    private static readonly Random Random = new();
    private static readonly string[] ProductNames = 
    {
        "Air Handler", "Heat Pump", "Ventilation Unit", "Climate Controller",
        "Fan Coil", "Chiller", "Compressor", "Condenser", "Evaporator"
    };
    
    private static readonly string[] Categories = 
    {
        "HVAC", "Ventilation", "Cooling", "Heating", "Control Systems"
    };

    /// <summary>
    /// Generates a random product
    /// </summary>
    public static Product GenerateRandomProduct()
    {
        var type = (ProductType)Random.Next(0, 4);
        return new ProductTestDataBuilder()
            .WithId(Guid.NewGuid())
            .WithName(GetRandomProductName())
            .WithDescription(GenerateRandomDescription())
            .WithType(type)
            .WithSku(GenerateRandomSku())
            .WithPrice(GenerateRandomPrice(type))
            .WithIsActive(Random.Next(0, 10) > 1) // 90% active
            .WithCategory(GetRandomCategory())
            .Build();
    }

    /// <summary>
    /// Generates multiple random products
    /// </summary>
    public static List<Product> GenerateRandomProducts(int count)
    {
        var products = new List<Product>();
        for (int i = 0; i < count; i++)
        {
            products.Add(GenerateRandomProduct());
        }
        return products;
    }

    /// <summary>
    /// Generates a random calculation
    /// </summary>
    public static Calculation GenerateRandomCalculation(Guid? productId = null)
    {
        var pid = productId ?? Guid.NewGuid();
        var subtotal = GenerateRandomDecimal(10, 10000);
        var discount = subtotal * GenerateRandomDecimal(0, 0.3m);
        var afterDiscount = subtotal - discount;
        var tax = afterDiscount * GenerateRandomDecimal(0.05m, 0.25m);
        var total = afterDiscount + tax;

        return new CalculationTestDataBuilder()
            .WithId(Guid.NewGuid())
            .WithProductId(pid)
            .WithStatus(GetRandomStatus())
            .WithSubtotal(subtotal)
            .WithDiscountAmount(discount)
            .WithTaxAmount(tax)
            .WithTotal(total)
            .WithCurrency(GetRandomCurrency())
            .Build();
    }

    /// <summary>
    /// Generates multiple random calculations
    /// </summary>
    public static List<Calculation> GenerateRandomCalculations(int count, Guid? productId = null)
    {
        var calculations = new List<Calculation>();
        for (int i = 0; i < count; i++)
        {
            calculations.Add(GenerateRandomCalculation(productId));
        }
        return calculations;
    }

    /// <summary>
    /// Generates a random product name
    /// </summary>
    public static string GetRandomProductName()
    {
        return ProductNames[Random.Next(ProductNames.Length)] + " " + Random.Next(100, 999);
    }

    /// <summary>
    /// Generates a random description
    /// </summary>
    public static string GenerateRandomDescription()
    {
        var descriptions = new[]
        {
            "High-efficiency unit with advanced features",
            "Industrial-grade equipment for heavy-duty applications",
            "Premium quality with extended warranty",
            "Cost-effective solution for standard applications",
            "Energy-efficient design with smart controls"
        };
        return descriptions[Random.Next(descriptions.Length)];
    }

    /// <summary>
    /// Generates a random SKU
    /// </summary>
    public static string GenerateRandomSku()
    {
        return $"SKU-{Random.Next(1000, 9999)}-{Random.Next(100, 999)}";
    }

    /// <summary>
    /// Generates a random price based on product type
    /// </summary>
    public static decimal GenerateRandomPrice(ProductType type)
    {
        return type switch
        {
            ProductType.Standard => GenerateRandomDecimal(50, 500),
            ProductType.Premium => GenerateRandomDecimal(500, 2000),
            ProductType.Custom => GenerateRandomDecimal(300, 1500),
            ProductType.Industrial => GenerateRandomDecimal(1000, 5000),
            _ => GenerateRandomDecimal(100, 1000)
        };
    }

    /// <summary>
    /// Generates a random category
    /// </summary>
    public static string GetRandomCategory()
    {
        return Categories[Random.Next(Categories.Length)];
    }

    /// <summary>
    /// Generates a random calculation status
    /// </summary>
    public static CalculationStatus GetRandomStatus()
    {
        var values = Enum.GetValues<CalculationStatus>();
        return values[Random.Next(values.Length)];
    }

    /// <summary>
    /// Generates a random currency code
    /// </summary>
    public static string GetRandomCurrency()
    {
        var currencies = new[] { "USD", "EUR", "GBP", "SEK", "NOK", "DKK" };
        return currencies[Random.Next(currencies.Length)];
    }

    /// <summary>
    /// Generates a random decimal in range
    /// </summary>
    public static decimal GenerateRandomDecimal(decimal min, decimal max)
    {
        var range = max - min;
        var sample = (decimal)Random.NextDouble();
        return min + (sample * range);
    }

    /// <summary>
    /// Generates a random integer in range
    /// </summary>
    public static int GenerateRandomInt(int min, int max)
    {
        return Random.Next(min, max + 1);
    }

    /// <summary>
    /// Generates a random boolean
    /// </summary>
    public static bool GenerateRandomBool()
    {
        return Random.Next(0, 2) == 1;
    }

    /// <summary>
    /// Generates random parameters
    /// </summary>
    public static Dictionary<string, object> GenerateRandomParameters()
    {
        var parameters = new Dictionary<string, object>
        {
            { "discount", GenerateRandomDecimal(0, 0.5m) },
            { "tax", GenerateRandomDecimal(0.05m, 0.25m) },
            { "applyBulkDiscount", GenerateRandomBool() },
            { "includeShipping", GenerateRandomBool() }
        };

        if (GenerateRandomBool())
        {
            parameters["priority"] = GenerateRandomInt(1, 5);
        }

        return parameters;
    }

    /// <summary>
    /// Generates a random date in the past
    /// </summary>
    public static DateTime GenerateRandomPastDate(int maxDaysAgo = 365)
    {
        var daysAgo = Random.Next(0, maxDaysAgo);
        return DateTime.UtcNow.AddDays(-daysAgo);
    }

    /// <summary>
    /// Generates a random date in the future
    /// </summary>
    public static DateTime GenerateRandomFutureDate(int maxDaysAhead = 365)
    {
        var daysAhead = Random.Next(1, maxDaysAhead);
        return DateTime.UtcNow.AddDays(daysAhead);
    }

    /// <summary>
    /// Generates random specifications
    /// </summary>
    public static Dictionary<string, string> GenerateRandomSpecifications()
    {
        return new Dictionary<string, string>
        {
            { "Weight", $"{GenerateRandomDecimal(5, 100):F2} kg" },
            { "Dimensions", $"{GenerateRandomInt(20, 100)}x{GenerateRandomInt(20, 100)}x{GenerateRandomInt(20, 100)} cm" },
            { "Power", $"{GenerateRandomInt(100, 5000)} W" },
            { "Voltage", $"{GenerateRandomInt(110, 240)} V" },
            { "Efficiency", $"{GenerateRandomInt(80, 99)}%" }
        };
    }

    /// <summary>
    /// Generates a batch of test data for integration tests
    /// </summary>
    public static (List<Product> Products, List<Calculation> Calculations) GenerateBatchTestData(int productCount, int calculationsPerProduct)
    {
        var products = GenerateRandomProducts(productCount);
        var calculations = new List<Calculation>();

        foreach (var product in products)
        {
            for (int i = 0; i < calculationsPerProduct; i++)
            {
                calculations.Add(GenerateRandomCalculation(product.Id));
            }
        }

        return (products, calculations);
    }
}
