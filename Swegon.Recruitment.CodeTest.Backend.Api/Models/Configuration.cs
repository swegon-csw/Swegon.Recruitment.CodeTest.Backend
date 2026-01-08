using System.ComponentModel.DataAnnotations;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

/// <summary>
/// Configuration model for system settings and product configurations
/// </summary>
public class Configuration : EntityModel
{
    /// <summary>
    /// Configuration name/key
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Configuration description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Type of configuration
    /// </summary>
    public ConfigurationType Type { get; set; }
    
    /// <summary>
    /// Configuration value (JSON serialized)
    /// </summary>
    [Required]
    public string Value { get; set; } = string.Empty;
    
    /// <summary>
    /// Category for grouping configurations
    /// </summary>
    [StringLength(50)]
    public string Category { get; set; } = "General";
    
    /// <summary>
    /// Whether this configuration is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Whether this configuration is read-only
    /// </summary>
    public bool IsReadOnly { get; set; }
    
    /// <summary>
    /// Environment this configuration applies to (Production, Staging, Development)
    /// </summary>
    [StringLength(50)]
    public string Environment { get; set; } = "Production";
    
    /// <summary>
    /// Data type of the value
    /// </summary>
    [StringLength(50)]
    public string DataType { get; set; } = "String";
    
    /// <summary>
    /// Validation rules for the configuration value
    /// </summary>
    public Dictionary<string, object>? ValidationRules { get; set; }
    
    /// <summary>
    /// Tags for categorization
    /// </summary>
    public List<string> Tags { get; set; } = new();
    
    /// <summary>
    /// Version of the configuration
    /// </summary>
    public int Version { get; set; } = 1;
    
    /// <summary>
    /// Previous value before last update
    /// </summary>
    public string? PreviousValue { get; set; }
    
    /// <summary>
    /// Effective date for this configuration
    /// </summary>
    public DateTime? EffectiveDate { get; set; }
    
    /// <summary>
    /// Expiration date for this configuration
    /// </summary>
    public DateTime? ExpirationDate { get; set; }
}
