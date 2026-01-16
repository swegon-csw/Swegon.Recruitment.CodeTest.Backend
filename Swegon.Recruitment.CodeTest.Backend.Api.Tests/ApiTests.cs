using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts;
using Swegon.Recruitment.CodeTest.Backend.Api.Services;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests;

public class ApiTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly CalculationService calculationService = new();

    [Fact]
    public async Task GetProducts_ReturnsSuccessAndCorrectContentType()
    {
        var response = await _client.GetAsync("/api/products");

        response.EnsureSuccessStatusCode();
        Assert.Equal(
            "application/json; charset=utf-8",
            response.Content.Headers.ContentType?.ToString()
        );
    }

    [Fact]
    public async Task GetProducts_ReturnsProductList()
    {
        var response = await _client.GetFromJsonAsync<List<ProductDto>>("/api/products");

        Assert.NotNull(response);
        Assert.IsType<List<ProductDto>>(response);
    }

    [Fact]
    public async Task Calculate_WithValidInput_ReturnsResult()
    {
        var input = new CalculationInputDto
        {
            Area = 100,
            Height = 3,
            Occupancy = 10,
            ActivityLevel = "medium",
            Temperature = 22,
            HumidityPercent = 50,
        };

        var response = await _client.PostAsJsonAsync("/api/calculations", input);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CalculationResultDto>();
        Assert.NotNull(result);
        Assert.True(result.Airflow > 0);
    }

    [Fact]
    public async Task Calculate_WithInvalidInput_ReturnsBadRequest()
    {
        var input = new CalculationInputDto { Area = -100, Height = 3 };

        var response = await _client.PostAsJsonAsync("/api/calculations", input);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/api/health");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", content);
    }

    [Fact]
    public async Task CalculateV2_WithValidInput_ReturnsResult()
    {
        var input = new CalculationInputDto
        {
            Area = 100,
            Height = 3,
            Occupancy = 10,
            ActivityLevel = "medium",
            Temperature = 22,
            HumidityPercent = 50,
        };

        var result = calculationService.CalculateAdvanced(input);

        Assert.NotNull(result);
        Assert.Equal(0.75, result.Airflow);
        Assert.Equal(15000, result.CoolingCapacity);
        Assert.Equal(12000, result.HeatingCapacity);
        Assert.Equal(18.9, result.EnergyConsumption);
        Assert.Equal(5000, result.Cost.Installation);
        Assert.Equal(1035, result.Cost.Annual);
        Assert.Equal(15522, result.Cost.Lifetime);
    }

    [Fact]
    public async Task CalculateV2_WithLowActivityLevel_ReturnsLowerValues()
    {
        var input = new CalculationInputDto
        {
            Area = 50,
            Height = 2.5,
            Occupancy = 5,
            ActivityLevel = "low",
            Temperature = 20,
            HumidityPercent = 45,
        };

        var result = calculationService.CalculateAdvanced(input);

        Assert.NotNull(result);
        Assert.Equal(0.21, result.Airflow);
        Assert.Equal(5000, result.CoolingCapacity);
        Assert.Equal(4000, result.HeatingCapacity);
        Assert.Equal(6.3, result.EnergyConsumption);
    }

    [Fact]
    public async Task CalculateV2_WithHighActivityLevel_ReturnsHigherValues()
    {
        var input = new CalculationInputDto
        {
            Area = 80,
            Height = 3.5,
            Occupancy = 15,
            ActivityLevel = "high",
            Temperature = 28,
            HumidityPercent = 65,
        };

        var result = calculationService.CalculateAdvanced(input);

        Assert.NotNull(result);
        Assert.Equal(0.93, result.Airflow);
        Assert.Equal(16000, result.CoolingCapacity);
        Assert.Equal(12800, result.HeatingCapacity);
        Assert.Equal(20.16, result.EnergyConsumption);
        Assert.Contains("humidity", result.Recommendations[0].ToLower());
    }

    [Fact]
    public async Task CalculateV2_WithHighTemperature_IncludesTemperatureRecommendation()
    {
        var input = new CalculationInputDto
        {
            Area = 60,
            Height = 3,
            Occupancy = 8,
            ActivityLevel = "medium",
            Temperature = 30,
            HumidityPercent = 50,
        };

        var result = calculationService.CalculateAdvanced(input);

        Assert.NotNull(result);
    }
}
