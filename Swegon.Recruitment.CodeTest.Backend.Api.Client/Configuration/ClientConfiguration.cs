using System;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Client.Configuration;

/// <summary>
/// Configuration settings for the API client.
/// </summary>
public class ClientConfiguration
{
    /// <summary>
    /// Gets or sets the base URL of the API.
    /// </summary>
    public string BaseUrl { get; set; } = "https://localhost:5001";

    /// <summary>
    /// Gets or sets the API key for authentication.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the request timeout in seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets or sets a value indicating whether to enable logging.
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of concurrent connections.
    /// </summary>
    public int MaxConcurrentConnections { get; set; } = 10;

    /// <summary>
    /// Gets or sets a value indicating whether to throw on API errors.
    /// </summary>
    public bool ThrowOnApiError { get; set; } = true;

    /// <summary>
    /// Gets or sets the user agent string.
    /// </summary>
    public string UserAgent { get; set; } = "SwegonApiClient/1.0";

    /// <summary>
    /// Gets or sets a value indicating whether to validate SSL certificates.
    /// </summary>
    public bool ValidateSslCertificate { get; set; } = true;

    /// <summary>
    /// Gets or sets the buffer size for streaming operations in bytes.
    /// </summary>
    public int StreamBufferSize { get; set; } = 81920; // 80KB

    /// <summary>
    /// Gets the request timeout as a TimeSpan.
    /// </summary>
    public TimeSpan Timeout => TimeSpan.FromSeconds(TimeoutSeconds);

    /// <summary>
    /// Validates the configuration.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when configuration is invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new InvalidOperationException("BaseUrl cannot be null or empty.");
        }

        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException($"BaseUrl '{BaseUrl}' is not a valid absolute URI.");
        }

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException($"BaseUrl must use HTTP or HTTPS scheme. Got: {uri.Scheme}");
        }

        if (TimeoutSeconds <= 0)
        {
            throw new InvalidOperationException("TimeoutSeconds must be greater than 0.");
        }

        if (MaxRetries < 0)
        {
            throw new InvalidOperationException("MaxRetries cannot be negative.");
        }

        if (MaxConcurrentConnections <= 0)
        {
            throw new InvalidOperationException("MaxConcurrentConnections must be greater than 0.");
        }

        if (StreamBufferSize <= 0)
        {
            throw new InvalidOperationException("StreamBufferSize must be greater than 0.");
        }
    }

    /// <summary>
    /// Creates a default configuration.
    /// </summary>
    /// <returns>A new <see cref="ClientConfiguration"/> instance with default values.</returns>
    public static ClientConfiguration Default() => new();

    /// <summary>
    /// Creates a configuration for development environments.
    /// </summary>
    /// <param name="baseUrl">The base URL.</param>
    /// <returns>A new <see cref="ClientConfiguration"/> instance configured for development.</returns>
    public static ClientConfiguration Development(string baseUrl = "https://localhost:5001")
    {
        return new ClientConfiguration
        {
            BaseUrl = baseUrl,
            ValidateSslCertificate = false,
            EnableLogging = true,
            TimeoutSeconds = 60,
            MaxRetries = 1
        };
    }

    /// <summary>
    /// Creates a configuration for production environments.
    /// </summary>
    /// <param name="baseUrl">The base URL.</param>
    /// <param name="apiKey">The API key.</param>
    /// <returns>A new <see cref="ClientConfiguration"/> instance configured for production.</returns>
    public static ClientConfiguration Production(string baseUrl, string apiKey)
    {
        return new ClientConfiguration
        {
            BaseUrl = baseUrl,
            ApiKey = apiKey,
            ValidateSslCertificate = true,
            EnableLogging = false,
            TimeoutSeconds = 30,
            MaxRetries = 3
        };
    }
}
