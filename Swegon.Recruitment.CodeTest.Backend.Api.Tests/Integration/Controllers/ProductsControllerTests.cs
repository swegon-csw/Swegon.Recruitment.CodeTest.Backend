using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.Fixtures;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.TestData;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Integration.Controllers;

/// <summary>
/// Integration tests for ProductsController
/// </summary>
[Collection("Integration Tests")]
public class ProductsControllerTests : IDisposable
{
    private readonly WebApplicationFactoryFixture _factory;
    private readonly HttpClient _client;

    public ProductsControllerTests(WebApplicationFactoryFixture factory)
    {
        _factory = factory;
        _client = _factory.CreateDefaultClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/products");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetProducts_ReturnsProductList()
    {
        var response = await _client.GetAsync("/api/products");
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        products.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProductById_ValidId_ReturnsProduct()
    {
        var createRequest = new ProductRequest
        {
            Name = "Test Product",
            Price = 100m,
            Type = ProductType.Standard
        };
        var createResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();

        var response = await _client.GetAsync($"/api/products/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProductById_InvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/products/{invalidId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateProduct_ValidRequest_ReturnsCreated()
    {
        var request = new ProductRequest
        {
            Name = "New Product",
            Description = "Test Description",
            Price = 150m,
            Type = ProductType.Standard
        };

        var response = await _client.PostAsJsonAsync("/api/products", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();
        product.Should().NotBeNull();
        product!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task CreateProduct_InvalidRequest_ReturnsBadRequest()
    {
        var request = new ProductRequest
        {
            Name = "",
            Price = -10m,
            Type = ProductType.Standard
        };

        var response = await _client.PostAsJsonAsync("/api/products", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProduct_ValidRequest_ReturnsOk()
    {
        var createRequest = new ProductRequest
        {
            Name = "Original Product",
            Price = 100m,
            Type = ProductType.Standard
        };
        var createResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();

        var updateRequest = new ProductRequest
        {
            Name = "Updated Product",
            Price = 200m,
            Type = ProductType.Premium
        };

        var response = await _client.PutAsJsonAsync($"/api/products/{created!.Id}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProduct_InvalidId_ReturnsNotFound()
    {
        var request = new ProductRequest
        {
            Name = "Product",
            Price = 100m,
            Type = ProductType.Standard
        };

        var response = await _client.PutAsJsonAsync($"/api/products/{Guid.NewGuid()}", request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProduct_ValidId_ReturnsNoContent()
    {
        var createRequest = new ProductRequest
        {
            Name = "Product to Delete",
            Price = 100m,
            Type = ProductType.Standard
        };
        var createResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();

        var response = await _client.DeleteAsync($"/api/products/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteProduct_InvalidId_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync($"/api/products/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProducts_WithFilters_ReturnsFilteredProducts()
    {
        var response = await _client.GetAsync("/api/products?type=Standard&isActive=true");
        response.EnsureSuccessStatusCode();
        
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        products.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProducts_WithPagination_ReturnsCorrectPage()
    {
        var response = await _client.GetAsync("/api/products?page=1&pageSize=10");
        response.EnsureSuccessStatusCode();
        
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        products.Should().NotBeNull();
    }

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}
