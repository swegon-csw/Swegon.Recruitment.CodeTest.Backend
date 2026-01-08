using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

/// <summary>
/// Response for configuration operations
/// </summary>
public class ConfigurationResponse
{
    /// <summary>
    /// Configuration ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Configuration key
    /// </summary>
    public string Key { get; set; } = string.Empty;
    
    /// <summary>
    /// Configuration value
    /// </summary>
    public string Value { get; set; } = string.Empty;
    
    /// <summary>
    /// Configuration type
    /// </summary>
    public ConfigurationType Type { get; set; }
    
    /// <summary>
    /// Configuration description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Whether the configuration is sensitive
    /// </summary>
    public bool IsSensitive { get; set; }
    
    /// <summary>
    /// Category for grouping configurations
    /// </summary>
    public string? Category { get; set; }
    
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
