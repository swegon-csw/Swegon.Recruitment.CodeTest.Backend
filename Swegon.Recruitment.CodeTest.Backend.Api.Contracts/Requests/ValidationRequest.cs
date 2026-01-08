using System.ComponentModel.DataAnnotations;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

/// <summary>
/// Request for validation operations
/// </summary>
public class ValidationRequest
{
    /// <summary>
    /// Data to validate
    /// </summary>
    [Required]
    public Dictionary<string, object> Data { get; set; } = new();
    
    /// <summary>
    /// Validation rules to apply
    /// </summary>
    public List<string>? Rules { get; set; }
    
    /// <summary>
    /// Whether to stop on first error
    /// </summary>
    public bool StopOnFirstError { get; set; }
    
    /// <summary>
    /// Context for validation
    /// </summary>
    public string? Context { get; set; }
}
