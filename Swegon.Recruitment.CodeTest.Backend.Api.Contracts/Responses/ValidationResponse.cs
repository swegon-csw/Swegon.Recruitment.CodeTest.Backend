namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

/// <summary>
/// Response for validation operations
/// </summary>
public class ValidationResponse
{
    /// <summary>
    /// Whether validation passed
    /// </summary>
    public bool IsValid { get; set; }
    
    /// <summary>
    /// Validation errors
    /// </summary>
    public List<ValidationError> Errors { get; set; } = new();
    
    /// <summary>
    /// Validation warnings
    /// </summary>
    public List<ValidationWarning> Warnings { get; set; } = new();
    
    /// <summary>
    /// Validated data
    /// </summary>
    public Dictionary<string, object>? ValidatedData { get; set; }
}

/// <summary>
/// Validation error details
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Field that failed validation
    /// </summary>
    public string Field { get; set; } = string.Empty;
    
    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Error code
    /// </summary>
    public string? Code { get; set; }
    
    /// <summary>
    /// Attempted value
    /// </summary>
    public object? AttemptedValue { get; set; }
}

/// <summary>
/// Validation warning details
/// </summary>
public class ValidationWarning
{
    /// <summary>
    /// Field with warning
    /// </summary>
    public string Field { get; set; } = string.Empty;
    
    /// <summary>
    /// Warning message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
