using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swegon.Recruitment.CodeTest.Backend.Api.Client.Clients;
using Swegon.Recruitment.CodeTest.Backend.Api.Client.Configuration;
using Swegon.Recruitment.CodeTest.Backend.Api.Client.Handlers;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Client.Extensions;

/// <summary>
/// Extension methods for configuring API clients in dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the API client and all sub-clients to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The client configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSwegonApiClient(
        this IServiceCollection services,
        ClientConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        configuration.Validate();

        // Register configuration
        services.AddSingleton(configuration);

        // Register HTTP client with handlers
        services.AddHttpClient<ApiClient>(client =>
        {
            client.BaseAddress = new Uri(configuration.BaseUrl);
            client.Timeout = configuration.Timeout;
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
                MaxConnectionsPerServer = configuration.MaxConcurrentConnections
            };

            if (!configuration.ValidateSslCertificate)
            {
                handler.SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => true
                };
            }

            return handler;
        })
        .AddHttpMessageHandler(sp => new AuthenticationHandler(configuration))
        .AddHttpMessageHandler(sp =>
        {
            if (!configuration.EnableLogging)
            {
                return new DelegatingHandler();
            }

            var logger = sp.GetRequiredService<ILogger<LoggingHandler>>();
            return new LoggingHandler(logger, logRequestBody: false, logResponseBody: false);
        });

        // Register individual clients
        services.AddScoped<IProductClient, ProductClient>();
        services.AddScoped<ICalculationClient, CalculationClient>();
        services.AddScoped<IConfigurationClient, ConfigurationClient>();

        return services;
    }

    /// <summary>
    /// Adds the API client with a configuration action.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">The configuration action.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSwegonApiClient(
        this IServiceCollection services,
        Action<ClientConfiguration> configureOptions)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configureOptions == null)
        {
            throw new ArgumentNullException(nameof(configureOptions));
        }

        var configuration = new ClientConfiguration();
        configureOptions(configuration);

        return services.AddSwegonApiClient(configuration);
    }

    /// <summary>
    /// Adds the API client with default configuration for development.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseUrl">The base URL.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSwegonApiClientDevelopment(
        this IServiceCollection services,
        string baseUrl = "https://localhost:5001")
    {
        return services.AddSwegonApiClient(ClientConfiguration.Development(baseUrl));
    }

    /// <summary>
    /// Adds the API client with configuration for production.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseUrl">The base URL.</param>
    /// <param name="apiKey">The API key.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSwegonApiClientProduction(
        this IServiceCollection services,
        string baseUrl,
        string apiKey)
    {
        return services.AddSwegonApiClient(ClientConfiguration.Production(baseUrl, apiKey));
    }

    /// <summary>
    /// Placeholder delegating handler for when logging is disabled.
    /// </summary>
    private class DelegatingHandler : System.Net.Http.DelegatingHandler
    {
    }
}
