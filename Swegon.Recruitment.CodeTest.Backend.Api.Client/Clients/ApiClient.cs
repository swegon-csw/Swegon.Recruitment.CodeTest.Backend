using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Swegon.Recruitment.CodeTest.Backend.Api.Client.Configuration;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Client.Clients;

/// <summary>
/// Main API client that aggregates all sub-clients for the Swegon Recruitment API.
/// </summary>
public class ApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ClientConfiguration _configuration;
    private bool _disposed;

    /// <summary>
    /// Gets the product client for interacting with product endpoints.
    /// </summary>
    public IProductClient Products { get; }

    /// <summary>
    /// Gets the calculation client for interacting with calculation endpoints.
    /// </summary>
    public ICalculationClient Calculations { get; }

    /// <summary>
    /// Gets the configuration client for interacting with configuration endpoints.
    /// </summary>
    public IConfigurationClient Configuration { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client (injected by DI).</param>
    /// <param name="configuration">The client configuration.</param>
    public ApiClient(HttpClient httpClient, ClientConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        // Initialize sub-clients
        Products = new ProductClient(_httpClient, _configuration);
        Calculations = new CalculationClient(_httpClient, _configuration);
        Configuration = new ConfigurationClient(_httpClient, _configuration);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClient"/> class with a base URL.
    /// </summary>
    /// <param name="baseUrl">The base URL of the API.</param>
    public ApiClient(string baseUrl)
        : this(baseUrl, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClient"/> class with a base URL and API key.
    /// </summary>
    /// <param name="baseUrl">The base URL of the API.</param>
    /// <param name="apiKey">The API key for authentication.</param>
    public ApiClient(string baseUrl, string? apiKey)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentException("Base URL cannot be null or empty.", nameof(baseUrl));
        }

        _configuration = new ClientConfiguration
        {
            BaseUrl = baseUrl,
            ApiKey = apiKey
        };

        _configuration.Validate();

        // Create HTTP client with handlers
        var handler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(10),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
            MaxConnectionsPerServer = _configuration.MaxConcurrentConnections
        };

        if (!_configuration.ValidateSslCertificate)
        {
            handler.SslOptions = new System.Net.Security.SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => true
            };
        }

        var authHandler = new Handlers.AuthenticationHandler(_configuration, handler);

        _httpClient = new HttpClient(authHandler)
        {
            BaseAddress = new Uri(_configuration.BaseUrl),
            Timeout = _configuration.Timeout
        };

        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        // Initialize sub-clients
        Products = new ProductClient(_httpClient, _configuration);
        Calculations = new CalculationClient(_httpClient, _configuration);
        Configuration = new ConfigurationClient(_httpClient, _configuration);
    }

    /// <summary>
    /// Checks the health status of the API.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The health status response.</returns>
    public async Task<HealthResponse> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("api/health", cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<HealthResponse>(cancellationToken: cancellationToken)
                .ConfigureAwait(false) ?? new HealthResponse { Status = "Unknown" };
        }
        catch (Exception ex)
        {
            return new HealthResponse
            {
                Status = "Unhealthy",
                Message = ex.Message
            };
        }
    }

    /// <summary>
    /// Pings the API to check connectivity.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the API is reachable; otherwise, false.</returns>
    public async Task<bool> PingAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("api/health", cancellationToken).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the API version information.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The API version information.</returns>
    public async Task<ApiVersionResponse> GetVersionAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("api/version", cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ApiVersionResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false) ?? new ApiVersionResponse { Version = "Unknown" };
    }

    /// <summary>
    /// Disposes the API client and its resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the API client and its resources.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }

            _disposed = true;
        }
    }
}

/// <summary>
/// Response model for API version information (internal use).
/// </summary>
internal class ApiVersionResponse
{
    public string Version { get; set; } = string.Empty;
    public string? BuildDate { get; set; }
    public string? Environment { get; set; }
}
