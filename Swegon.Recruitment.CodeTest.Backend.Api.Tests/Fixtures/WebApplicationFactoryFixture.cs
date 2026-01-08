using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Fixtures;

/// <summary>
/// Custom WebApplicationFactory for integration tests
/// </summary>
public class WebApplicationFactoryFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace services for testing if needed
            // Example: Replace database with in-memory version
            
            // Configure logging
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Warning);
            });
        });

        builder.UseEnvironment("Testing");
    }

    /// <summary>
    /// Creates a client with default configuration
    /// </summary>
    public HttpClient CreateDefaultClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("http://localhost")
        });
    }

    /// <summary>
    /// Creates a client with custom configuration
    /// </summary>
    public HttpClient CreateClientWithOptions(WebApplicationFactoryClientOptions options)
    {
        return CreateClient(options);
    }
}

/// <summary>
/// Fixture collection for sharing WebApplicationFactory across tests
/// </summary>
[CollectionDefinition("Integration Tests")]
public class IntegrationTestCollection : ICollectionFixture<WebApplicationFactoryFixture>
{
}
