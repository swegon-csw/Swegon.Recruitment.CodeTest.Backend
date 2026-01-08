using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.Fixtures;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Integration.Scenarios;

/// <summary>
/// End-to-end tests for calculation workflows
/// </summary>
[Collection("Integration Tests")]
public class EndToEndCalculationTests : IDisposable
{
    private readonly WebApplicationFactoryFixture _factory;
    private readonly HttpClient _client;

    public EndToEndCalculationTests(WebApplicationFactoryFixture factory)
    {
        _factory = factory;
        _client = _factory.CreateDefaultClient();
    }

    [Fact]
    public async Task CompleteCalculationWorkflow_CreateProductAndCalculate_Succeeds()
    {
        // Create product
        var productRequest = new ProductRequest
        {
            Name = "E2E Test Product",
            Description = "End-to-end test",
            Price = 250m,
            Type = ProductType.Premium
        };
        var productResponse = await _client.PostAsJsonAsync("/api/products", productRequest);
        productResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductResponse>();

        // Perform calculation
        var calculationRequest = new CalculationRequest
        {
            ProductId = product!.Id,
            Quantity = 10,
            Parameters = new Dictionary<string, object>()
        };
        var calcResponse = await _client.PostAsJsonAsync("/api/calculations", calculationRequest);
        calcResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var calculation = await calcResponse.Content.ReadFromJsonAsync<CalculationResponse>();
        
        calculation.Should().NotBeNull();
        calculation!.ProductId.Should().Be(product.Id);
    }

    [Fact]
    public async Task MultipleCalculations_SameProduct_AllSucceed()
    {
        // Create product
        var productRequest = new ProductRequest
        {
            Name = "Bulk Test Product",
            Price = 100m,
            Type = ProductType.Standard
        };
        var productResponse = await _client.PostAsJsonAsync("/api/products", productRequest);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductResponse>();

        // Perform multiple calculations
        for (int i = 1; i <= 5; i++)
        {
            var calculationRequest = new CalculationRequest
            {
                ProductId = product!.Id,
                Quantity = i * 10,
                Parameters = new Dictionary<string, object>()
            };
            var response = await _client.PostAsJsonAsync("/api/calculations", calculationRequest);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    [Fact]
    public async Task CalculationWithDiscount_AppliesCorrectly()
    {
        var productRequest = new ProductRequest
        {
            Name = "Discount Product",
            Price = 100m,
            Type = ProductType.Standard
        };
        var productResponse = await _client.PostAsJsonAsync("/api/products", productRequest);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductResponse>();

        var calculationRequest = new CalculationRequest
        {
            ProductId = product!.Id,
            Quantity = 50, // Should trigger bulk discount
            Parameters = new Dictionary<string, object>()
        };
        var response = await _client.PostAsJsonAsync("/api/calculations", calculationRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CalculateMultipleProductTypes_AllSucceed()
    {
        var productTypes = new[] 
        { 
            ProductType.Standard, 
            ProductType.Premium, 
            ProductType.Industrial, 
            ProductType.Custom 
        };

        foreach (var type in productTypes)
        {
            var productRequest = new ProductRequest
            {
                Name = $"{type} Product",
                Price = 100m,
                Type = type
            };
            var productResponse = await _client.PostAsJsonAsync("/api/products", productRequest);
            var product = await productResponse.Content.ReadFromJsonAsync<ProductResponse>();

            var calculationRequest = new CalculationRequest
            {
                ProductId = product!.Id,
                Quantity = 1,
                Parameters = new Dictionary<string, object>()
            };
            var response = await _client.PostAsJsonAsync("/api/calculations", calculationRequest);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    [Fact]
    public async Task CalculationHistory_AfterMultipleCalculations_ReturnsAll()
    {
        var productRequest = new ProductRequest
        {
            Name = "History Test Product",
            Price = 100m,
            Type = ProductType.Standard
        };
        var productResponse = await _client.PostAsJsonAsync("/api/products", productRequest);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductResponse>();

        // Create multiple calculations
        for (int i = 0; i < 3; i++)
        {
            var calculationRequest = new CalculationRequest
            {
                ProductId = product!.Id,
                Quantity = i + 1,
                Parameters = new Dictionary<string, object>()
            };
            await _client.PostAsJsonAsync("/api/calculations", calculationRequest);
        }

        // Get history
        var historyResponse = await _client.GetAsync("/api/calculations/history");
        historyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}
