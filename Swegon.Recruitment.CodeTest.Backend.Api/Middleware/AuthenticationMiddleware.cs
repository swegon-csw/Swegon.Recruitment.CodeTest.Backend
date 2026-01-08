namespace Swegon.Recruitment.CodeTest.Backend.Api.Middleware;

/// <summary>
/// Middleware for authentication
/// </summary>
public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationMiddleware> _logger;

    public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check for API key in header
        if (!context.Request.Headers.TryGetValue("X-API-Key", out var apiKey))
        {
            // For now, allow all requests (authentication disabled for demo)
            await _next(context);
            return;
        }

        // Validate API key (simplified for demo)
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API key");
            return;
        }

        _logger.LogDebug("Request authenticated with API key");
        await _next(context);
    }
}
