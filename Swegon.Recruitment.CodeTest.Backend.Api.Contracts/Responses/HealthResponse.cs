namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

/// <summary>
/// Response for health check operations
/// </summary>
public class HealthResponse
{
    /// <summary>
    /// Overall health status
    /// </summary>
    public string Status { get; set; } = "Healthy";
    
    /// <summary>
    /// Individual component health checks
    /// </summary>
    public Dictionary<string, ComponentHealth> Components { get; set; } = new();
    
    /// <summary>
    /// Application version
    /// </summary>
    public string Version { get; set; } = "1.0.0";
    
    /// <summary>
    /// Timestamp of health check
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Application uptime
    /// </summary>
    public TimeSpan Uptime { get; set; }
}

/// <summary>
/// Health status of a component
/// </summary>
public class ComponentHealth
{
    /// <summary>
    /// Component status
    /// </summary>
    public string Status { get; set; } = "Healthy";
    
    /// <summary>
    /// Component description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Response time in milliseconds
    /// </summary>
    public long? ResponseTimeMs { get; set; }
    
    /// <summary>
    /// Additional component data
    /// </summary>
    public Dictionary<string, object>? Data { get; set; }
}
