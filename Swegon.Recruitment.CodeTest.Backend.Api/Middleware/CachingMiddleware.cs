using Microsoft.Extensions.Caching.Memory;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Middleware;

/// <summary>
/// Middleware for response caching
/// </summary>
public class CachingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingMiddleware> _logger;

    public CachingMiddleware(
        RequestDelegate next,
        IMemoryCache cache,
        ILogger<CachingMiddleware> logger)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only cache GET requests
        if (context.Request.Method != HttpMethods.Get)
        {
            await _next(context);
            return;
        }

        var cacheKey = $"response_{context.Request.Path}{context.Request.QueryString}";

        if (_cache.TryGetValue(cacheKey, out byte[] cachedResponse))
        {
            _logger.LogDebug("Returning cached response for {Path}", context.Request.Path);
            await context.Response.Body.WriteAsync(cachedResponse);
            return;
        }

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        if (context.Response.StatusCode == 200)
        {
            var responseBytes = responseBody.ToArray();
            _cache.Set(cacheKey, responseBytes, TimeSpan.FromMinutes(5));
            
            await responseBody.CopyToAsync(originalBodyStream);
        }
        else
        {
            context.Response.Body = originalBodyStream;
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
