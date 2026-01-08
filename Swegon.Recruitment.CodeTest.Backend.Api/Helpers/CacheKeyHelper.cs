using System.Security.Cryptography;
using System.Text;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Helpers;

/// <summary>
/// Helper class for generating cache keys
/// </summary>
public static class CacheKeyHelper
{
    private const string Separator = ":";
    
    /// <summary>
    /// Generates a cache key for a product
    /// </summary>
    public static string GetProductKey(Guid productId)
    {
        return $"product{Separator}{productId}";
    }
    
    /// <summary>
    /// Generates a cache key for all products
    /// </summary>
    public static string GetAllProductsKey(int page, int pageSize)
    {
        return $"products{Separator}page{Separator}{page}{Separator}size{Separator}{pageSize}";
    }
    
    /// <summary>
    /// Generates a cache key for product search
    /// </summary>
    public static string GetProductSearchKey(string searchTerm, int page, int pageSize)
    {
        var hash = ComputeHash(searchTerm);
        return $"products{Separator}search{Separator}{hash}{Separator}page{Separator}{page}{Separator}size{Separator}{pageSize}";
    }
    
    /// <summary>
    /// Generates a cache key for a calculation
    /// </summary>
    public static string GetCalculationKey(Guid calculationId)
    {
        return $"calculation{Separator}{calculationId}";
    }
    
    /// <summary>
    /// Generates a cache key for calculation history
    /// </summary>
    public static string GetCalculationHistoryKey(Guid productId, int page, int pageSize)
    {
        return $"calculations{Separator}product{Separator}{productId}{Separator}page{Separator}{page}{Separator}size{Separator}{pageSize}";
    }
    
    /// <summary>
    /// Generates a cache key for a configuration
    /// </summary>
    public static string GetConfigurationKey(string name)
    {
        return $"config{Separator}{name}";
    }
    
    /// <summary>
    /// Generates a cache key for all configurations
    /// </summary>
    public static string GetAllConfigurationsKey(string category)
    {
        return $"configs{Separator}{category}";
    }
    
    /// <summary>
    /// Generates a cache key with custom parameters
    /// </summary>
    public static string GenerateKey(string prefix, params object[] parameters)
    {
        var keyParts = new List<string> { prefix };
        
        foreach (var param in parameters)
        {
            if (param != null)
            {
                keyParts.Add(param.ToString() ?? string.Empty);
            }
        }
        
        return string.Join(Separator, keyParts);
    }
    
    /// <summary>
    /// Generates a cache key from a dictionary of parameters
    /// </summary>
    public static string GenerateKeyFromDictionary(string prefix, Dictionary<string, object> parameters)
    {
        if (parameters == null || parameters.Count == 0)
            return prefix;
        
        var sortedParams = parameters.OrderBy(kvp => kvp.Key).Select(kvp => $"{kvp.Key}={kvp.Value}");
        var paramString = string.Join("&", sortedParams);
        var hash = ComputeHash(paramString);
        
        return $"{prefix}{Separator}{hash}";
    }
    
    /// <summary>
    /// Generates a wildcard pattern for cache key matching
    /// </summary>
    public static string GetWildcardPattern(string prefix)
    {
        return $"{prefix}{Separator}*";
    }
    
    /// <summary>
    /// Invalidates cache keys with a specific prefix
    /// </summary>
    public static List<string> GetInvalidationPattern(string prefix)
    {
        return new List<string> { GetWildcardPattern(prefix) };
    }
    
    /// <summary>
    /// Computes an MD5 hash of a string
    /// </summary>
    private static string ComputeHash(string input)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        
        var sb = new StringBuilder();
        foreach (var b in hashBytes)
        {
            sb.Append(b.ToString("x2"));
        }
        
        return sb.ToString();
    }
    
    /// <summary>
    /// Validates a cache key format
    /// </summary>
    public static bool IsValidKey(string key)
    {
        return !string.IsNullOrWhiteSpace(key) && key.Length <= 250 && !key.Contains(' ');
    }
    
    /// <summary>
    /// Extracts the prefix from a cache key
    /// </summary>
    public static string ExtractPrefix(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;
        
        var separatorIndex = key.IndexOf(Separator);
        return separatorIndex > 0 ? key.Substring(0, separatorIndex) : key;
    }
    
    /// <summary>
    /// Generates a user-specific cache key
    /// </summary>
    public static string GetUserSpecificKey(string userId, string resource, params object[] parameters)
    {
        var keyParts = new List<object> { "user", userId, resource };
        keyParts.AddRange(parameters);
        
        return GenerateKey("cache", keyParts.ToArray());
    }
    
    /// <summary>
    /// Generates a time-based cache key (includes current hour)
    /// </summary>
    public static string GetTimeBasedKey(string prefix, DateTime timestamp)
    {
        var hourKey = timestamp.ToString("yyyyMMddHH");
        return $"{prefix}{Separator}{hourKey}";
    }
}
