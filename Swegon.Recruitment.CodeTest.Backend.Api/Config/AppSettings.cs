namespace Swegon.Recruitment.CodeTest.Backend.Api.Config;

/// <summary>
/// Application settings configuration
/// </summary>
public class AppSettings
{
    /// <summary>
    /// API configuration
    /// </summary>
    public ApiConfiguration Api { get; set; } = new();
    
    /// <summary>
    /// Cache configuration
    /// </summary>
    public CacheConfiguration Cache { get; set; } = new();
    
    /// <summary>
    /// External services configuration
    /// </summary>
    public ExternalServicesConfiguration ExternalServices { get; set; } = new();
    
    /// <summary>
    /// Feature flags
    /// </summary>
    public Dictionary<string, bool> Features { get; set; } = new();
}

/// <summary>
/// API-specific configuration
/// </summary>
public class ApiConfiguration
{
    /// <summary>
    /// API version
    /// </summary>
    public string Version { get; set; } = "1.0";
    
    /// <summary>
    /// API title
    /// </summary>
    public string Title { get; set; } = "Swegon Recruitment CodeTest API";
    
    /// <summary>
    /// Maximum request size in bytes
    /// </summary>
    public long MaxRequestSize { get; set; } = 10485760; // 10MB
    
    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}

/// <summary>
/// External services configuration
/// </summary>
public class ExternalServicesConfiguration
{
    /// <summary>
    /// External API endpoints
    /// </summary>
    public Dictionary<string, string> Endpoints { get; set; } = new();
    
    /// <summary>
    /// API keys for external services
    /// </summary>
    public Dictionary<string, string> ApiKeys { get; set; } = new();
}
