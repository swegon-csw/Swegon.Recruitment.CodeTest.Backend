using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Client.Handlers;

/// <summary>
/// DelegatingHandler that logs HTTP requests and responses.
/// </summary>
public class LoggingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHandler> _logger;
    private readonly bool _logRequestBody;
    private readonly bool _logResponseBody;
    private readonly int _maxBodyLogLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="logRequestBody">Whether to log request bodies.</param>
    /// <param name="logResponseBody">Whether to log response bodies.</param>
    /// <param name="maxBodyLogLength">Maximum length of body content to log.</param>
    public LoggingHandler(
        ILogger<LoggingHandler> logger,
        bool logRequestBody = false,
        bool logResponseBody = false,
        int maxBodyLogLength = 4096)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logRequestBody = logRequestBody;
        _logResponseBody = logResponseBody;
        _maxBodyLogLength = maxBodyLogLength;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="innerHandler">The inner handler.</param>
    /// <param name="logRequestBody">Whether to log request bodies.</param>
    /// <param name="logResponseBody">Whether to log response bodies.</param>
    /// <param name="maxBodyLogLength">Maximum length of body content to log.</param>
    public LoggingHandler(
        ILogger<LoggingHandler> logger,
        HttpMessageHandler innerHandler,
        bool logRequestBody = false,
        bool logResponseBody = false,
        int maxBodyLogLength = 4096)
        : base(innerHandler)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logRequestBody = logRequestBody;
        _logResponseBody = logResponseBody;
        _maxBodyLogLength = maxBodyLogLength;
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

        var correlationId = GetCorrelationId(request);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Log request
            await LogRequestAsync(request, correlationId, cancellationToken).ConfigureAwait(false);

            // Send request
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            stopwatch.Stop();

            // Log response
            await LogResponseAsync(response, correlationId, stopwatch.ElapsedMilliseconds, cancellationToken)
                .ConfigureAwait(false);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "[{CorrelationId}] Request failed after {ElapsedMs}ms: {Method} {Uri}",
                correlationId,
                stopwatch.ElapsedMilliseconds,
                request.Method,
                request.RequestUri);
            throw;
        }
    }

    /// <summary>
    /// Logs the HTTP request.
    /// </summary>
    private async Task LogRequestAsync(
        HttpRequestMessage request,
        string correlationId,
        CancellationToken cancellationToken)
    {
        var requestLog = new StringBuilder();
        requestLog.AppendLine($"[{correlationId}] HTTP Request:");
        requestLog.AppendLine($"  {request.Method} {request.RequestUri}");
        requestLog.AppendLine($"  Headers:");

        foreach (var header in request.Headers)
        {
            // Don't log sensitive headers
            var headerName = header.Key;
            var headerValue = IsSensitiveHeader(headerName)
                ? "***REDACTED***"
                : string.Join(", ", header.Value);

            requestLog.AppendLine($"    {headerName}: {headerValue}");
        }

        if (_logRequestBody && request.Content != null)
        {
            var body = await ReadBodyAsync(request.Content, cancellationToken).ConfigureAwait(false);
            requestLog.AppendLine($"  Body: {body}");
        }

        _logger.LogInformation(requestLog.ToString());
    }

    /// <summary>
    /// Logs the HTTP response.
    /// </summary>
    private async Task LogResponseAsync(
        HttpResponseMessage response,
        string correlationId,
        long elapsedMilliseconds,
        CancellationToken cancellationToken)
    {
        var responseLog = new StringBuilder();
        responseLog.AppendLine($"[{correlationId}] HTTP Response ({elapsedMilliseconds}ms):");
        responseLog.AppendLine($"  Status: {(int)response.StatusCode} {response.StatusCode}");
        responseLog.AppendLine($"  Headers:");

        foreach (var header in response.Headers)
        {
            responseLog.AppendLine($"    {header.Key}: {string.Join(", ", header.Value)}");
        }

        if (_logResponseBody && response.Content != null)
        {
            var body = await ReadBodyAsync(response.Content, cancellationToken).ConfigureAwait(false);
            responseLog.AppendLine($"  Body: {body}");
        }

        var logLevel = response.IsSuccessStatusCode ? LogLevel.Information : LogLevel.Warning;
        _logger.Log(logLevel, responseLog.ToString());
    }

    /// <summary>
    /// Reads the HTTP content body.
    /// </summary>
    private async Task<string> ReadBodyAsync(HttpContent content, CancellationToken cancellationToken)
    {
        try
        {
            var body = await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (body.Length > _maxBodyLogLength)
            {
                return body.Substring(0, _maxBodyLogLength) + $"... ({body.Length - _maxBodyLogLength} more characters)";
            }

            return body;
        }
        catch (Exception ex)
        {
            return $"[Error reading body: {ex.Message}]";
        }
    }

    /// <summary>
    /// Gets the correlation ID from the request.
    /// </summary>
    private static string GetCorrelationId(HttpRequestMessage request)
    {
        if (request.Options.TryGetValue(new HttpRequestOptionsKey<string>("CorrelationId"), out var correlationId))
        {
            return correlationId;
        }

        if (request.Headers.TryGetValues("X-Correlation-ID", out var values))
        {
            return string.Join(", ", values);
        }

        return Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Determines whether a header contains sensitive information.
    /// </summary>
    private static bool IsSensitiveHeader(string headerName)
    {
        var lowerHeaderName = headerName.ToLowerInvariant();
        return lowerHeaderName == "authorization" ||
               lowerHeaderName == "x-api-key" ||
               lowerHeaderName == "cookie" ||
               lowerHeaderName == "set-cookie";
    }
}
