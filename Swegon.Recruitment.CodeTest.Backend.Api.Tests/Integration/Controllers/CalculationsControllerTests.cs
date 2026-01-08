using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.Fixtures;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Integration.Controllers;

/// <summary>
/// Integration tests for CalculationsController
/// </summary>
[Collection("Integration Tests")]
public class CalculationsControllerTests : IDisposable
{
    private readonly WebApplicationFactoryFixture _factory;
    private readonly HttpClient _client;

    public CalculationsControllerTests(WebApplicationFactoryFixture factory)
    {
        _factory = factory;
        _client = _factory.CreateDefaultClient();
    }

    [Fact]
    public async Task Calculate_ValidRequest_ReturnsCalculation()
    {
        var productRequest = new ProductRequest
        {
            Name = "Test Product",
            Price = 100m,
            Type = ProductType.Standard
        };
        var productResponse = await _client.PostAsJsonAsync("/api/products", productRequest);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductResponse>();

        var calculationRequest = new CalculationRequest
        {
            ProductId = product!.Id,
            Quantity = 5,
            Parameters = new Dictionary<string, object>()
        };

        var response = await _client.PostAsJsonAsync("/api/calculations", calculationRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var calculation = await response.Content.ReadFromJsonAsync<CalculationResponse>();
        calculation.Should().NotBeNull();
    }

    [Fact]
    public async Task Calculate_InvalidProductId_ReturnsBadRequest()
    {
        var request = new CalculationRequest
        {
            ProductId = Guid.NewGuid(),
            Quantity = 5,
            Parameters = new Dictionary<string, object>()
        };

        var response = await _client.PostAsJsonAsync("/api/calculations", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Calculate_InvalidQuantity_ReturnsBadRequest(int quantity)
    {
        var request = new CalculationRequest
        {
            ProductId = Guid.NewGuid(),
            Quantity = quantity,
            Parameters = new Dictionary<string, object>()
        };

        var response = await _client.PostAsJsonAsync("/api/calculations", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCalculationHistory_ReturnsHistory()
    {
        var response = await _client.GetAsync("/api/calculations/history");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetCalculationHistory_WithProductId_ReturnsFilteredHistory()
    {
        var productId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/calculations/history?productId={productId}");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Calculate_WithParameters_AppliesParameters()
    {
        var productRequest = new ProductRequest
        {
            Name = "Test Product",
            Price = 100m,
            Type = ProductType.Standard
        };
        var productResponse = await _client.PostAsJsonAsync("/api/products", productRequest);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductResponse>();

        var calculationRequest = new CalculationRequest
        {
            ProductId = product!.Id,
            Quantity = 1,
            Parameters = new Dictionary<string, object>
            {
                { "discount", 0.1 },
                { "tax", 0.25 }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/calculations", calculationRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Calculate_MultipleCalculations_AllSucceed()
    {
        var productRequest = new ProductRequest
        {
            Name = "Test Product",
            Price = 100m,
            Type = ProductType.Standard
        };
        var productResponse = await _client.PostAsJsonAsync("/api/products", productRequest);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductResponse>();

        for (int i = 1; i <= 5; i++)
        {
            var calculationRequest = new CalculationRequest
            {
                ProductId = product!.Id,
                Quantity = i,
                Parameters = new Dictionary<string, object>()
            };

            var response = await _client.PostAsJsonAsync("/api/calculations", calculationRequest);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}
