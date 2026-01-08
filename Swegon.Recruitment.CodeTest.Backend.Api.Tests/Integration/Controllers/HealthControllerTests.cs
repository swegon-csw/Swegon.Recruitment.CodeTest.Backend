using System.Net;
using FluentAssertions;
using Swegon.Recruitment.CodeTest.Backend.Api.Tests.Fixtures;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Integration.Controllers;

/// <summary>
/// Integration tests for HealthController
/// </summary>
[Collection("Integration Tests")]
public class HealthControllerTests : IDisposable
{
    private readonly WebApplicationFactoryFixture _factory;
    private readonly HttpClient _client;

    public HealthControllerTests(WebApplicationFactoryFixture factory)
    {
        _factory = factory;
        _client = _factory.CreateDefaultClient();
    }

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/api/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Health_ReturnsJsonContent()
    {
        var response = await _client.GetAsync("/api/health");
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Health_MultipleCalls_AllSucceed()
    {
        for (int i = 0; i < 10; i++)
        {
            var response = await _client.GetAsync("/api/health");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}
