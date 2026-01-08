using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Swegon.Recruitment.CodeTest.Backend.Api.Client.Configuration;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Client.Handlers;

/// <summary>
/// DelegatingHandler that adds authentication headers to requests.
/// </summary>
public class AuthenticationHandler : DelegatingHandler
{
    private readonly ClientConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationHandler"/> class.
    /// </summary>
    /// <param name="configuration">The client configuration.</param>
    public AuthenticationHandler(ClientConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationHandler"/> class.
    /// </summary>
    /// <param name="configuration">The client configuration.</param>
    /// <param name="innerHandler">The inner handler.</param>
    public AuthenticationHandler(ClientConfiguration configuration, HttpMessageHandler innerHandler)
        : base(innerHandler)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Add API key if configured
        if (!string.IsNullOrWhiteSpace(_configuration.ApiKey))
        {
            AddApiKeyHeader(request);
        }

        // Add user agent
        if (!string.IsNullOrWhiteSpace(_configuration.UserAgent))
        {
            AddUserAgent(request);
        }

        // Add correlation ID for request tracking
        AddCorrelationId(request);

        // Add timestamp
        AddTimestamp(request);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds the API key to the request headers.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    private void AddApiKeyHeader(HttpRequestMessage request)
    {
        // Option 1: Bearer token in Authorization header
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _configuration.ApiKey);

        // Option 2: Custom header (uncomment if preferred)
        // request.Headers.Add("X-API-Key", _configuration.ApiKey);
    }

    /// <summary>
    /// Adds the user agent to the request headers.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    private void AddUserAgent(HttpRequestMessage request)
    {
        if (!request.Headers.UserAgent.TryParseAdd(_configuration.UserAgent))
        {
            // Fallback: add as custom header if parsing fails
            request.Headers.Add("User-Agent", _configuration.UserAgent);
        }
    }

    /// <summary>
    /// Adds a correlation ID to the request headers for tracking.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    private void AddCorrelationId(HttpRequestMessage request)
    {
        var correlationId = Guid.NewGuid().ToString();
        request.Headers.Add("X-Correlation-ID", correlationId);

        // Store in request properties for logging
        request.Options.Set(new HttpRequestOptionsKey<string>("CorrelationId"), correlationId);
    }

    /// <summary>
    /// Adds a timestamp to the request headers.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    private void AddTimestamp(HttpRequestMessage request)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        request.Headers.Add("X-Request-Timestamp", timestamp);
    }
}
