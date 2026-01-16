using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

public class ValidationResult
{
    public bool IsValid { get; set; }

    public List<ValidationError> Errors { get; set; } = new();

    public List<ValidationWarning> Warnings { get; set; } = new();

    public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;

    public string ValidatorName { get; set; } = string.Empty;

    public Dictionary<string, object> Context { get; set; } = new();

    public static ValidationResult Valid() => new() { IsValid = true };

    public static ValidationResult Invalid(params ValidationError[] errors)
    {
        return new ValidationResult { IsValid = false, Errors = new List<ValidationError>(errors) };
    }

    public void AddError(
        string field,
        string message,
        ErrorCode errorCode = ErrorCode.ValidationError
    )
    {
        Errors.Add(
            new ValidationError
            {
                Field = field,
                Message = message,
                ErrorCode = errorCode,
            }
        );
        IsValid = false;
    }

    public void AddWarning(string field, string message)
    {
        Warnings.Add(new ValidationWarning { Field = field, Message = message });
    }

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

public class ValidationError
{
    public string Field { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public ErrorCode ErrorCode { get; set; }

    public object? AttemptedValue { get; set; }

    public Dictionary<string, object>? Details { get; set; }
}

public class ValidationWarning
{
    public string Field { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string Severity { get; set; } = "Medium";
}
