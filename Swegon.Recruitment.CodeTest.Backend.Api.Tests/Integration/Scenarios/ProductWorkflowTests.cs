using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.Fixtures;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Integration.Scenarios;

/// <summary>
/// End-to-end tests for product workflows
/// </summary>
[Collection("Integration Tests")]
public class ProductWorkflowTests : IDisposable
{
    private readonly WebApplicationFactoryFixture _factory;
    private readonly HttpClient _client;

    public ProductWorkflowTests(WebApplicationFactoryFixture factory)
    {
        _factory = factory;
        _client = _factory.CreateDefaultClient();
    }

    [Fact]
    public async Task CompleteProductLifecycle_CreateReadUpdateDelete_Succeeds()
    {
        // Create
        var createRequest = new ProductRequest
        {
            Name = "Lifecycle Product",
            Description = "Test lifecycle",
            Price = 150m,
            Type = ProductType.Standard
        };
        var createResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();

        // Read
        var readResponse = await _client.GetAsync($"/api/products/{created!.Id}");
        readResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Update
        var updateRequest = new ProductRequest
        {
            Name = "Updated Lifecycle Product",
            Price = 200m,
            Type = ProductType.Premium
        };
        var updateResponse = await _client.PutAsJsonAsync($"/api/products/{created.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Delete
        var deleteResponse = await _client.DeleteAsync($"/api/products/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CreateMultipleProducts_ThenQueryAll_ReturnsAll()
    {
        var products = new List<ProductRequest>
        {
            new() { Name = "Product 1", Price = 100m, Type = ProductType.Standard },
            new() { Name = "Product 2", Price = 200m, Type = ProductType.Premium },
            new() { Name = "Product 3", Price = 300m, Type = ProductType.Industrial }
        };

        foreach (var product in products)
        {
            var response = await _client.PostAsJsonAsync("/api/products", product);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        var listResponse = await _client.GetAsync("/api/products");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProduct_MultipleFields_AllUpdated()
    {
        var createRequest = new ProductRequest
        {
            Name = "Original Product",
            Description = "Original Description",
            Price = 100m,
            Type = ProductType.Standard
        };
        var createResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();

        var updateRequest = new ProductRequest
        {
            Name = "Updated Name",
            Description = "Updated Description",
            Price = 250m,
            Type = ProductType.Premium
        };
        var updateResponse = await _client.PutAsJsonAsync($"/api/products/{created!.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var updated = await updateResponse.Content.ReadFromJsonAsync<ProductResponse>();
        updated!.Name.Should().Be("Updated Name");
        updated.Description.Should().Be("Updated Description");
    }

    [Fact]
    public async Task FilterProducts_ByType_ReturnsCorrectProducts()
    {
        var standardProduct = new ProductRequest
        {
            Name = "Standard Product",
            Price = 100m,
            Type = ProductType.Standard
        };
        var premiumProduct = new ProductRequest
        {
            Name = "Premium Product",
            Price = 500m,
            Type = ProductType.Premium
        };

        await _client.PostAsJsonAsync("/api/products", standardProduct);
        await _client.PostAsJsonAsync("/api/products", premiumProduct);

        var response = await _client.GetAsync("/api/products?type=Premium");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PaginateProducts_ReturnsCorrectPages()
    {
        // Create multiple products
        for (int i = 0; i < 15; i++)
        {
            var product = new ProductRequest
            {
                Name = $"Paginated Product {i}",
                Price = 100m + i,
                Type = ProductType.Standard
            };
            await _client.PostAsJsonAsync("/api/products", product);
        }

        var page1 = await _client.GetAsync("/api/products?page=1&pageSize=10");
        var page2 = await _client.GetAsync("/api/products?page=2&pageSize=10");

        page1.StatusCode.Should().Be(HttpStatusCode.OK);
        page2.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}
