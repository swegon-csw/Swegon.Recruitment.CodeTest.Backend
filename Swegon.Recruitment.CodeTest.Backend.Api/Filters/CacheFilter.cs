using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Filters;

/// <summary>
/// Filter for response caching
/// </summary>
public class CacheFilter : IActionFilter
{
    private readonly IMemoryCache _cache;
    private readonly int _durationSeconds;

    public CacheFilter(IMemoryCache cache, int durationSeconds = 300)
    {
        _cache = cache;
        _durationSeconds = durationSeconds;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var cacheKey = GenerateCacheKey(context);
        
        if (_cache.TryGetValue(cacheKey, out object? cachedResult))
        {
            context.Result = new OkObjectResult(cachedResult);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is OkObjectResult okResult)
        {
            var cacheKey = GenerateCacheKey(context);
            _cache.Set(cacheKey, okResult.Value, TimeSpan.FromSeconds(_durationSeconds));
        }
    }

    private static string GenerateCacheKey(FilterContext context)
    {
        var request = context.HttpContext.Request;
        return $"{request.Path}{request.QueryString}";
    }
}
