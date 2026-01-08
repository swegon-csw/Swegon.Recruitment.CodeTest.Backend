using System;
using System.Net;
using System.Net.Http;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Client.Configuration;

/// <summary>
/// Configuration for HttpClient instances.
/// </summary>
public class HttpClientConfig
{
    /// <summary>
    /// Gets or sets the maximum response content buffer size in bytes.
    /// </summary>
    public long MaxResponseContentBufferSize { get; set; } = 10485760; // 10MB

    /// <summary>
    /// Gets or sets a value indicating whether to use HTTP/2.
    /// </summary>
    public bool UseHttp2 { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of connections per server.
    /// </summary>
    public int MaxConnectionsPerServer { get; set; } = 10;

    /// <summary>
    /// Gets or sets a value indicating whether to allow automatic redirects.
    /// </summary>
    public bool AllowAutoRedirect { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of automatic redirects.
    /// </summary>
    public int MaxAutomaticRedirections { get; set; } = 5;

    /// <summary>
    /// Gets or sets a value indicating whether to use cookies.
    /// </summary>
    public bool UseCookies { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to use default credentials.
    /// </summary>
    public bool UseDefaultCredentials { get; set; } = false;

    /// <summary>
    /// Gets or sets the decompression methods.
    /// </summary>
    public DecompressionMethods DecompressionMethods { get; set; } = 
        DecompressionMethods.GZip | DecompressionMethods.Deflate;

    /// <summary>
    /// Gets or sets the connection idle timeout in seconds.
    /// </summary>
    public int PooledConnectionIdleTimeoutSeconds { get; set; } = 120;

    /// <summary>
    /// Gets or sets the connection lifetime in seconds.
    /// </summary>
    public int PooledConnectionLifetimeSeconds { get; set; } = 600;

    /// <summary>
    /// Gets or sets a value indicating whether to expect 100-Continue responses.
    /// </summary>
    public bool Expect100Continue { get; set; } = false;

    /// <summary>
    /// Gets the pooled connection idle timeout as a TimeSpan.
    /// </summary>
    public TimeSpan PooledConnectionIdleTimeout => 
        TimeSpan.FromSeconds(PooledConnectionIdleTimeoutSeconds);

    /// <summary>
    /// Gets the pooled connection lifetime as a TimeSpan.
    /// </summary>
    public TimeSpan PooledConnectionLifetime => 
        TimeSpan.FromSeconds(PooledConnectionLifetimeSeconds);

    /// <summary>
    /// Applies the configuration to an HttpClientHandler.
    /// </summary>
    /// <param name="handler">The handler to configure.</param>
    public void ApplyTo(HttpClientHandler handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        handler.MaxConnectionsPerServer = MaxConnectionsPerServer;
        handler.AllowAutoRedirect = AllowAutoRedirect;
        handler.MaxAutomaticRedirections = MaxAutomaticRedirections;
        handler.UseCookies = UseCookies;
        handler.UseDefaultCredentials = UseDefaultCredentials;
        handler.AutomaticDecompression = DecompressionMethods;
    }

    /// <summary>
    /// Applies the configuration to a SocketsHttpHandler.
    /// </summary>
    /// <param name="handler">The handler to configure.</param>
    public void ApplyTo(SocketsHttpHandler handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        handler.MaxConnectionsPerServer = MaxConnectionsPerServer;
        handler.AllowAutoRedirect = AllowAutoRedirect;
        handler.MaxAutomaticRedirections = MaxAutomaticRedirections;
        handler.UseCookies = UseCookies;
        handler.UseDefaultCredentials = UseDefaultCredentials;
        handler.AutomaticDecompression = DecompressionMethods;
        handler.PooledConnectionIdleTimeout = PooledConnectionIdleTimeout;
        handler.PooledConnectionLifetime = PooledConnectionLifetime;
        handler.Expect100ContinueTimeout = Expect100Continue 
            ? TimeSpan.FromSeconds(1) 
            : TimeSpan.Zero;
    }

    /// <summary>
    /// Creates a default configuration.
    /// </summary>
    /// <returns>A new <see cref="HttpClientConfig"/> instance with default values.</returns>
    public static HttpClientConfig Default() => new();
}
