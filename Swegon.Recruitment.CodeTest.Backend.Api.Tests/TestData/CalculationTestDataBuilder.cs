using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.TestData;

/// <summary>
/// Fluent builder for creating test calculations
/// </summary>
public class CalculationTestDataBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _productId = Guid.NewGuid();
    private CalculationStatus _status = CalculationStatus.Completed;
    private decimal _total = 100.00m;
    private decimal _subtotal = 90.00m;
    private decimal _discountAmount = 10.00m;
    private decimal _taxAmount = 20.00m;
    private string _currency = "USD";
    private Dictionary<string, object> _metadata = new();
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;

    /// <summary>
    /// Sets the calculation ID
    /// </summary>
    public CalculationTestDataBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    /// <summary>
    /// Sets the product ID
    /// </summary>
    public CalculationTestDataBuilder WithProductId(Guid productId)
    {
        _productId = productId;
        return this;
    }

    /// <summary>
    /// Sets the status
    /// </summary>
    public CalculationTestDataBuilder WithStatus(CalculationStatus status)
    {
        _status = status;
        return this;
    }

    /// <summary>
    /// Sets the total
    /// </summary>
    public CalculationTestDataBuilder WithTotal(decimal total)
    {
        _total = total;
        return this;
    }

    /// <summary>
    /// Sets the subtotal
    /// </summary>
    public CalculationTestDataBuilder WithSubtotal(decimal subtotal)
    {
        _subtotal = subtotal;
        return this;
    }

    /// <summary>
    /// Sets the discount amount
    /// </summary>
    public CalculationTestDataBuilder WithDiscountAmount(decimal discountAmount)
    {
        _discountAmount = discountAmount;
        return this;
    }

    /// <summary>
    /// Sets the tax amount
    /// </summary>
    public CalculationTestDataBuilder WithTaxAmount(decimal taxAmount)
    {
        _taxAmount = taxAmount;
        return this;
    }

    /// <summary>
    /// Sets the currency
    /// </summary>
    public CalculationTestDataBuilder WithCurrency(string currency)
    {
        _currency = currency;
        return this;
    }

    /// <summary>
    /// Adds metadata
    /// </summary>
    public CalculationTestDataBuilder WithMetadata(string key, object value)
    {
        _metadata[key] = value;
        return this;
    }

    /// <summary>
    /// Sets as pending
    /// </summary>
    public CalculationTestDataBuilder AsPending()
    {
        _status = CalculationStatus.Pending;
        return this;
    }

    /// <summary>
    /// Sets as failed
    /// </summary>
    public CalculationTestDataBuilder AsFailed()
    {
        _status = CalculationStatus.Failed;
        return this;
    }

    /// <summary>
    /// Sets as completed
    /// </summary>
    public CalculationTestDataBuilder AsCompleted()
    {
        _status = CalculationStatus.Completed;
        return this;
    }

    /// <summary>
    /// Sets timestamps
    /// </summary>
    public CalculationTestDataBuilder WithTimestamps(DateTime createdAt, DateTime updatedAt)
    {
        _createdAt = createdAt;
        _updatedAt = updatedAt;
        return this;
    }

    /// <summary>
    /// Builds the calculation
    /// </summary>
    public Calculation Build()
    {
        return new Calculation
        {
            Id = _id,
            ProductId = _productId,
            Status = _status,
            Total = _total,
            Subtotal = _subtotal,
            DiscountAmount = _discountAmount,
            TaxAmount = _taxAmount,
            Currency = _currency,
            Metadata = _metadata,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt
        };
    }

    /// <summary>
    /// Creates a default calculation
    /// </summary>
    public static Calculation CreateDefaultCalculation()
    {
        return new CalculationTestDataBuilder().Build();
    }

    /// <summary>
    /// Creates a calculation with specific product
    /// </summary>
    public static Calculation CreateCalculationForProduct(Guid productId)
    {
        return new CalculationTestDataBuilder()
            .WithProductId(productId)
            .Build();
    }

    /// <summary>
    /// Creates multiple calculations
    /// </summary>
    public static List<Calculation> CreateMultipleCalculations(int count)
    {
        var calculations = new List<Calculation>();
        
        for (int i = 0; i < count; i++)
        {
            var builder = new CalculationTestDataBuilder()
                .WithId(Guid.NewGuid())
                .WithTotal(100 * (i + 1))
                .WithSubtotal(90 * (i + 1));
            
            calculations.Add(builder.Build());
        }
        
        return calculations;
    }

    /// <summary>
    /// Creates a calculation request
    /// </summary>
    public static CalculationRequest CreateCalculationRequest(Guid productId, int quantity = 1)
    {
        return new CalculationRequest
        {
            ProductId = productId,
            Quantity = quantity,
            Parameters = new Dictionary<string, object>()
        };
    }

    /// <summary>
    /// Creates a calculation request with parameters
    /// </summary>
    public static CalculationRequest CreateCalculationRequestWithParameters(
        Guid productId, 
        int quantity,
        Dictionary<string, object> parameters)
    {
        return new CalculationRequest
        {
            ProductId = productId,
            Quantity = quantity,
            Parameters = parameters
        };
    }

    /// <summary>
    /// Creates a calculation result
    /// </summary>
    public static CalculationResult CreateCalculationResult()
    {
        return new CalculationResult
        {
            ProductId = Guid.NewGuid(),
            Status = CalculationStatus.Completed,
            Total = 100.00m,
            Subtotal = 90.00m,
            DiscountAmount = 10.00m,
            TaxAmount = 20.00m,
            BasePrice = 100.00m,
            Quantity = 1,
            Metadata = new Dictionary<string, object>()
        };
    }

    /// <summary>
    /// Creates a failed calculation result
    /// </summary>
    public static CalculationResult CreateFailedCalculationResult(string errorMessage)
    {
        var result = new CalculationResult
        {
            ProductId = Guid.NewGuid(),
            Status = CalculationStatus.Failed,
            Metadata = new Dictionary<string, object>
            {
                { "Error", errorMessage }
            }
        };
        return result;
    }
}
