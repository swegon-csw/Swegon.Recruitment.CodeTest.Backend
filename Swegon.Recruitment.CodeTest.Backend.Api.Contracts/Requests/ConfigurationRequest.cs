using System.ComponentModel.DataAnnotations;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

/// <summary>
/// Request for creating or updating configuration
/// </summary>
public class ConfigurationRequest
{
    /// <summary>
    /// Configuration key
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Key { get; set; } = string.Empty;
    
    /// <summary>
    /// Configuration value
    /// </summary>
    [Required]
    public string Value { get; set; } = string.Empty;
    
    /// <summary>
    /// Configuration type
    /// </summary>
    public ConfigurationType Type { get; set; } = ConfigurationType.Global;
    
    /// <summary>
    /// Configuration description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Whether the configuration is sensitive
    /// </summary>
    public bool IsSensitive { get; set; }
    
    /// <summary>
    /// Category for grouping configurations
    /// </summary>
    [StringLength(50)]
    public string? Category { get; set; }
}
