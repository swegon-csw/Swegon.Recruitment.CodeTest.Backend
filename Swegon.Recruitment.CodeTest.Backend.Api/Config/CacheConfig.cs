namespace Swegon.Recruitment.CodeTest.Backend.Api.Config;

/// <summary>
/// Cache configuration settings
/// </summary>
public class CacheConfiguration
{
    /// <summary>
    /// Default cache duration in minutes
    /// </summary>
    public int DefaultDurationMinutes { get; set; } = 30;
    
    /// <summary>
    /// Maximum cache size in MB
    /// </summary>
    public int MaxSizeMb { get; set; } = 100;
    
    /// <summary>
    /// Whether caching is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// Cache expiration scanning frequency in minutes
    /// </summary>
    public int ExpirationScanFrequencyMinutes { get; set; } = 5;
    
    /// <summary>
    /// Specific cache durations for different entity types
    /// </summary>
    public Dictionary<string, int> EntityDurations { get; set; } = new()
    {
        { "Product", 60 },
        { "Calculation", 15 },
        { "Configuration", 120 }
    };
}
