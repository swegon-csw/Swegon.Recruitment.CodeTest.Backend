using FluentAssertions;
using Swegon.Recruitment.CodeTest.Backend.Api.Helpers;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.TestData;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Unit.Helpers;

/// <summary>
/// Unit tests for MappingHelper
/// </summary>
public class MappingHelperTests
{
    [Fact]
    public void MapToResponse_ValidProduct_ReturnsProductResponse()
    {
        var product = ProductTestDataBuilder.CreateStandardProduct();
        var response = MappingHelper.MapToResponse(product);
        response.Should().NotBeNull();
        response.Id.Should().Be(product.Id);
        response.Name.Should().Be(product.Name);
    }

    [Fact]
    public void MapToResponse_NullProduct_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => MappingHelper.MapToResponse(null!));
    }

    [Fact]
    public void MapToProduct_ValidRequest_ReturnsProduct()
    {
        var request = new ProductRequest
        {
            Name = "Test Product",
            Price = 100m,
            Type = Contracts.Enums.ProductType.Standard
        };
        var product = MappingHelper.MapToProduct(request);
        product.Should().NotBeNull();
        product.Name.Should().Be(request.Name);
    }

    [Fact]
    public void MapToProduct_NullRequest_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => MappingHelper.MapToProduct(null!));
    }

    [Fact]
    public void MapToCalculationResponse_ValidCalculation_ReturnsResponse()
    {
        var calculation = CalculationTestDataBuilder.CreateDefaultCalculation();
        var response = MappingHelper.MapToCalculationResponse(calculation);
        response.Should().NotBeNull();
        response.Id.Should().Be(calculation.Id);
    }

    [Fact]
    public void MapCollection_MultipleProducts_ReturnsMultipleResponses()
    {
        var products = ProductTestDataBuilder.CreateMultipleProducts(5);
        var responses = products.Select(MappingHelper.MapToResponse).ToList();
        responses.Should().HaveCount(5);
    }
}
