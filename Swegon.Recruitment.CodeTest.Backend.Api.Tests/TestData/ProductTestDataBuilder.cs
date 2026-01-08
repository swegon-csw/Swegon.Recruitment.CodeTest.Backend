using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.TestData;

/// <summary>
/// Fluent builder for creating test products
/// </summary>
public class ProductTestDataBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Test Product";
    private string? _description = "Test Description";
    private ProductType _type = ProductType.Standard;
    private string? _sku = "TEST-001";
    private decimal _price = 100.00m;
    private bool _isActive = true;
    private string? _category = "General";
    private Dictionary<string, string> _specifications = new();
    private Dictionary<string, object> _metadata = new();
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;
    private bool _isDeleted = false;

    /// <summary>
    /// Sets the product ID
    /// </summary>
    public ProductTestDataBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    /// <summary>
    /// Sets the product name
    /// </summary>
    public ProductTestDataBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the product description
    /// </summary>
    public ProductTestDataBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    /// <summary>
    /// Sets the product type
    /// </summary>
    public ProductTestDataBuilder WithType(ProductType type)
    {
        _type = type;
        return this;
    }

    /// <summary>
    /// Sets the SKU
    /// </summary>
    public ProductTestDataBuilder WithSku(string? sku)
    {
        _sku = sku;
        return this;
    }

    /// <summary>
    /// Sets the price
    /// </summary>
    public ProductTestDataBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    /// <summary>
    /// Sets whether product is active
    /// </summary>
    public ProductTestDataBuilder WithIsActive(bool isActive)
    {
        _isActive = isActive;
        return this;
    }

    /// <summary>
    /// Sets the category
    /// </summary>
    public ProductTestDataBuilder WithCategory(string? category)
    {
        _category = category;
        return this;
    }

    /// <summary>
    /// Adds a specification
    /// </summary>
    public ProductTestDataBuilder WithSpecification(string key, string value)
    {
        _specifications[key] = value;
        return this;
    }

    /// <summary>
    /// Adds metadata
    /// </summary>
    public ProductTestDataBuilder WithMetadata(string key, object value)
    {
        _metadata[key] = value;
        return this;
    }

    /// <summary>
    /// Sets as premium product
    /// </summary>
    public ProductTestDataBuilder AsPremium()
    {
        _type = ProductType.Premium;
        _price = 500.00m;
        return this;
    }

    /// <summary>
    /// Sets as industrial product
    /// </summary>
    public ProductTestDataBuilder AsIndustrial()
    {
        _type = ProductType.Industrial;
        _price = 1000.00m;
        return this;
    }

    /// <summary>
    /// Sets as custom product
    /// </summary>
    public ProductTestDataBuilder AsCustom()
    {
        _type = ProductType.Custom;
        _price = 750.00m;
        return this;
    }

    /// <summary>
    /// Sets as inactive
    /// </summary>
    public ProductTestDataBuilder AsInactive()
    {
        _isActive = false;
        return this;
    }

    /// <summary>
    /// Sets as deleted
    /// </summary>
    public ProductTestDataBuilder AsDeleted()
    {
        _isDeleted = true;
        return this;
    }

    /// <summary>
    /// Sets creation and update timestamps
    /// </summary>
    public ProductTestDataBuilder WithTimestamps(DateTime createdAt, DateTime updatedAt)
    {
        _createdAt = createdAt;
        _updatedAt = updatedAt;
        return this;
    }

    /// <summary>
    /// Builds the product
    /// </summary>
    public Product Build()
    {
        return new Product
        {
            Id = _id,
            Name = _name,
            Description = _description,
            Type = _type,
            SKU = _sku,
            Price = _price,
            IsActive = _isActive,
            Category = _category,
            Specifications = _specifications,
            Metadata = _metadata,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt,
            IsDeleted = _isDeleted
        };
    }

    /// <summary>
    /// Creates a default standard product
    /// </summary>
    public static Product CreateStandardProduct()
    {
        return new ProductTestDataBuilder().Build();
    }

    /// <summary>
    /// Creates a default premium product
    /// </summary>
    public static Product CreatePremiumProduct()
    {
        return new ProductTestDataBuilder().AsPremium().Build();
    }

    /// <summary>
    /// Creates a default industrial product
    /// </summary>
    public static Product CreateIndustrialProduct()
    {
        return new ProductTestDataBuilder().AsIndustrial().Build();
    }

    /// <summary>
    /// Creates a default custom product
    /// </summary>
    public static Product CreateCustomProduct()
    {
        return new ProductTestDataBuilder().AsCustom().Build();
    }

    /// <summary>
    /// Creates multiple products with different types
    /// </summary>
    public static List<Product> CreateMultipleProducts(int count)
    {
        var products = new List<Product>();
        var types = new[] { ProductType.Standard, ProductType.Premium, ProductType.Custom, ProductType.Industrial };
        
        for (int i = 0; i < count; i++)
        {
            var type = types[i % types.Length];
            var builder = new ProductTestDataBuilder()
                .WithId(Guid.NewGuid())
                .WithName($"Product {i + 1}")
                .WithSku($"SKU-{i + 1:000}")
                .WithType(type)
                .WithPrice(100 * (i + 1));
            
            products.Add(builder.Build());
        }
        
        return products;
    }

    /// <summary>
    /// Creates a product with complete specifications
    /// </summary>
    public static Product CreateProductWithSpecifications()
    {
        return new ProductTestDataBuilder()
            .WithSpecification("Weight", "10kg")
            .WithSpecification("Dimensions", "30x40x50cm")
            .WithSpecification("Material", "Steel")
            .WithSpecification("Color", "Silver")
            .Build();
    }

    /// <summary>
    /// Creates a minimal valid product
    /// </summary>
    public static Product CreateMinimalProduct()
    {
        return new ProductTestDataBuilder()
            .WithName("Min")
            .WithPrice(0.01m)
            .Build();
    }

    /// <summary>
    /// Creates a product with maximum values
    /// </summary>
    public static Product CreateMaxProduct()
    {
        return new ProductTestDataBuilder()
            .WithName(new string('A', 100))
            .WithDescription(new string('B', 500))
            .WithPrice(999999.99m)
            .Build();
    }
}
