using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Services;

/// <summary>
/// Memory cache service wrapper with expiration handling
/// </summary>
public class CacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheService> _logger;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);
    
    public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// Gets a value from cache
    /// </summary>
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key cannot be empty", nameof(key));
        
        try
        {
            if (_cache.TryGetValue(key, out T? value))
            {
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return value;
            }
            
            _logger.LogDebug("Cache miss for key: {Key}", key);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting value from cache for key: {Key}", key);
            return null;
        }
    }
    
    /// <summary>
    /// Sets a value in cache with expiration
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key cannot be empty", nameof(key));
        
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        
        try
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
            };
            
            _cache.Set(key, value, options);
            _logger.LogDebug("Cached value for key: {Key} with expiration: {Expiration}", 
                key, expiration ?? _defaultExpiration);
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value in cache for key: {Key}", key);
        }
    }
    
    /// <summary>
    /// Removes a value from cache
    /// </summary>
    public async Task RemoveAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key cannot be empty", nameof(key));
        
        try
        {
            _cache.Remove(key);
            _logger.LogDebug("Removed cache key: {Key}", key);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing value from cache for key: {Key}", key);
        }
    }
    
    /// <summary>
    /// Checks if a key exists in cache
    /// </summary>
    public bool Exists(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;
        
        return _cache.TryGetValue(key, out _);
    }
    
    /// <summary>
    /// Gets or creates a cache entry
    /// </summary>
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null) where T : class
    {
        var cached = await GetAsync<T>(key);
        if (cached != null)
            return cached;
        
        var value = await factory();
        await SetAsync(key, value, expiration);
        
        return value;
    }
}
