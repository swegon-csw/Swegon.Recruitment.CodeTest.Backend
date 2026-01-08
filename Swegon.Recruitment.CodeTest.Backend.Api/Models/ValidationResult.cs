using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

/// <summary>
/// Result of validation operations
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Whether validation passed
    /// </summary>
    public bool IsValid { get; set; }
    
    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<ValidationError> Errors { get; set; } = new();
    
    /// <summary>
    /// List of validation warnings
    /// </summary>
    public List<ValidationWarning> Warnings { get; set; } = new();
    
    /// <summary>
    /// Validation timestamp
    /// </summary>
    public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Validator that performed the validation
    /// </summary>
    public string ValidatorName { get; set; } = string.Empty;
    
    /// <summary>
    /// Additional validation context
    /// </summary>
    public Dictionary<string, object> Context { get; set; } = new();
    
    /// <summary>
    /// Creates a valid result
    /// </summary>
    public static ValidationResult Valid() => new() { IsValid = true };
    
    /// <summary>
    /// Creates an invalid result with errors
    /// </summary>
    public static ValidationResult Invalid(params ValidationError[] errors)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = new List<ValidationError>(errors)
        };
    }
    
    /// <summary>
    /// Adds an error to the validation result
    /// </summary>
    public void AddError(string field, string message, ErrorCode errorCode = ErrorCode.ValidationError)
    {
        Errors.Add(new ValidationError
        {
            Field = field,
            Message = message,
            ErrorCode = errorCode
        });
        IsValid = false;
    }
    
    /// <summary>
    /// Adds a warning to the validation result
    /// </summary>
    public void AddWarning(string field, string message)
    {
        Warnings.Add(new ValidationWarning
        {
            Field = field,
            Message = message
        });
    }
    
    /// <summary>
    /// Combines multiple validation results
    /// </summary>
    public static ValidationResult Combine(params ValidationResult[] results)
    {
        var combined = new ValidationResult { IsValid = true };
        
        foreach (var result in results)
        {
            if (!result.IsValid)
            {
                combined.IsValid = false;
            }
            combined.Errors.AddRange(result.Errors);
            combined.Warnings.AddRange(result.Warnings);
        }
        
        return combined;
    }
}

/// <summary>
/// Represents a validation error
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
    public ErrorCode ErrorCode { get; set; }
    
    /// <summary>
    /// Attempted value that failed validation
    /// </summary>
    public object? AttemptedValue { get; set; }
    
    /// <summary>
    /// Additional error details
    /// </summary>
    public Dictionary<string, object>? Details { get; set; }
}

/// <summary>
/// Represents a validation warning
/// </summary>
public class ValidationWarning
{
    /// <summary>
    /// Field that triggered the warning
    /// </summary>
    public string Field { get; set; } = string.Empty;
    
    /// <summary>
    /// Warning message
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Severity level (Low, Medium, High)
    /// </summary>
    public string Severity { get; set; } = "Medium";
}
