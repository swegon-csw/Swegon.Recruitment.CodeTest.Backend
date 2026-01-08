using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;
using System.Text.Json;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Validators;

/// <summary>
/// Validator for configuration operations
/// </summary>
public class ConfigurationValidator
{
    private static readonly string[] ValidEnvironments = { "Production", "Staging", "Development", "Test" };
    private static readonly string[] ValidDataTypes = { "String", "Integer", "Decimal", "Boolean", "Json", "Array" };
    
    /// <summary>
    /// Validates a configuration model
    /// </summary>
    public ValidationResult Validate(Configuration configuration)
    {
        var result = new ValidationResult { IsValid = true, ValidatorName = nameof(ConfigurationValidator) };
        
        ValidateName(configuration, result);
        ValidateValue(configuration, result);
        ValidateType(configuration, result);
        ValidateEnvironment(configuration, result);
        ValidateDataType(configuration, result);
        ValidateDates(configuration, result);
        ValidateVersion(configuration, result);
        
        return result;
    }
    
    /// <summary>
    /// Validates a configuration request
    /// </summary>
    public ValidationResult ValidateRequest(ConfigurationRequest request)
    {
        var result = new ValidationResult { IsValid = true, ValidatorName = nameof(ConfigurationValidator) };
        
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            result.AddError(nameof(request.Name), "Configuration name is required", ErrorCode.ValidationError);
        }
        else if (request.Name.Length > 100)
        {
            result.AddError(nameof(request.Name), "Configuration name cannot exceed 100 characters", ErrorCode.ValidationError);
        }
        
        if (string.IsNullOrWhiteSpace(request.Value))
        {
            result.AddError(nameof(request.Value), "Configuration value is required", ErrorCode.ValidationError);
        }
        
        if (request.Description?.Length > 500)
        {
            result.AddError(nameof(request.Description), "Description cannot exceed 500 characters", ErrorCode.ValidationError);
        }
        
        return result;
    }
    
    /// <summary>
    /// Validates configuration name
    /// </summary>
    private void ValidateName(Configuration configuration, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(configuration.Name))
        {
            result.AddError(nameof(configuration.Name), "Configuration name is required", ErrorCode.ValidationError);
            return;
        }
        
        if (configuration.Name.Length > 100)
        {
            result.AddError(nameof(configuration.Name), "Configuration name cannot exceed 100 characters", ErrorCode.ValidationError);
        }
        
        if (!System.Text.RegularExpressions.Regex.IsMatch(configuration.Name, @"^[A-Za-z0-9._-]+$"))
        {
            result.AddError(nameof(configuration.Name), "Configuration name can only contain alphanumeric characters, dots, underscores, and hyphens", ErrorCode.ValidationError);
        }
        
        if (configuration.Name.StartsWith("_") || configuration.Name.StartsWith("-"))
        {
            result.AddWarning(nameof(configuration.Name), "Configuration name should not start with special characters");
        }
    }
    
    /// <summary>
    /// Validates configuration value
    /// </summary>
    private void ValidateValue(Configuration configuration, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(configuration.Value))
        {
            result.AddError(nameof(configuration.Value), "Configuration value is required", ErrorCode.ValidationError);
            return;
        }
        
        if (configuration.Value.Length > 10000)
        {
            result.AddError(nameof(configuration.Value), "Configuration value is too large", ErrorCode.ValidationError);
        }
        
        // Validate value based on data type
        ValidateValueForDataType(configuration, result);
    }
    
    /// <summary>
    /// Validates value matches the specified data type
    /// </summary>
    private void ValidateValueForDataType(Configuration configuration, ValidationResult result)
    {
        switch (configuration.DataType)
        {
            case "Integer":
                if (!int.TryParse(configuration.Value, out _))
                {
                    result.AddError(nameof(configuration.Value), "Value must be a valid integer", ErrorCode.ValidationError);
                }
                break;
                
            case "Decimal":
                if (!decimal.TryParse(configuration.Value, out _))
                {
                    result.AddError(nameof(configuration.Value), "Value must be a valid decimal", ErrorCode.ValidationError);
                }
                break;
                
            case "Boolean":
                if (!bool.TryParse(configuration.Value, out _))
                {
                    result.AddError(nameof(configuration.Value), "Value must be a valid boolean (true/false)", ErrorCode.ValidationError);
                }
                break;
                
            case "Json":
                try
                {
                    JsonDocument.Parse(configuration.Value);
                }
                catch
                {
                    result.AddError(nameof(configuration.Value), "Value must be valid JSON", ErrorCode.ValidationError);
                }
                break;
        }
    }
    
    /// <summary>
    /// Validates configuration type
    /// </summary>
    private void ValidateType(Configuration configuration, ValidationResult result)
    {
        if (!Enum.IsDefined(typeof(ConfigurationType), configuration.Type))
        {
            result.AddError(nameof(configuration.Type), "Invalid configuration type", ErrorCode.ValidationError);
        }
    }
    
    /// <summary>
    /// Validates environment
    /// </summary>
    private void ValidateEnvironment(Configuration configuration, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(configuration.Environment))
        {
            result.AddError(nameof(configuration.Environment), "Environment is required", ErrorCode.ValidationError);
            return;
        }
        
        if (!ValidEnvironments.Contains(configuration.Environment, StringComparer.OrdinalIgnoreCase))
        {
            result.AddWarning(nameof(configuration.Environment), 
                $"Environment '{configuration.Environment}' is not a standard value. Expected: {string.Join(", ", ValidEnvironments)}");
        }
    }
    
    /// <summary>
    /// Validates data type
    /// </summary>
    private void ValidateDataType(Configuration configuration, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(configuration.DataType))
        {
            result.AddError(nameof(configuration.DataType), "Data type is required", ErrorCode.ValidationError);
            return;
        }
        
        if (!ValidDataTypes.Contains(configuration.DataType, StringComparer.OrdinalIgnoreCase))
        {
            result.AddWarning(nameof(configuration.DataType), 
                $"Data type '{configuration.DataType}' is not a standard type. Expected: {string.Join(", ", ValidDataTypes)}");
        }
    }
    
    /// <summary>
    /// Validates effective and expiration dates
    /// </summary>
    private void ValidateDates(Configuration configuration, ValidationResult result)
    {
        if (configuration.EffectiveDate.HasValue && configuration.EffectiveDate.Value > DateTime.UtcNow.AddYears(10))
        {
            result.AddWarning(nameof(configuration.EffectiveDate), "Effective date is far in the future");
        }
        
        if (configuration.ExpirationDate.HasValue && configuration.ExpirationDate.Value < DateTime.UtcNow)
        {
            result.AddWarning(nameof(configuration.ExpirationDate), "Configuration has already expired");
        }
        
        if (configuration.EffectiveDate.HasValue && configuration.ExpirationDate.HasValue)
        {
            if (configuration.EffectiveDate.Value >= configuration.ExpirationDate.Value)
            {
                result.AddError("Dates", "Effective date must be before expiration date", ErrorCode.ValidationError);
            }
            
            var dateRange = configuration.ExpirationDate.Value - configuration.EffectiveDate.Value;
            if (dateRange.TotalDays < 1)
            {
                result.AddWarning("Dates", "Configuration has a very short validity period");
            }
        }
    }
    
    /// <summary>
    /// Validates version
    /// </summary>
    private void ValidateVersion(Configuration configuration, ValidationResult result)
    {
        if (configuration.Version < 1)
        {
            result.AddError(nameof(configuration.Version), "Version must be at least 1", ErrorCode.ValidationError);
        }
        
        if (configuration.Version > 10000)
        {
            result.AddWarning(nameof(configuration.Version), "Version number is unusually high");
        }
    }
    
    /// <summary>
    /// Validates validation rules
    /// </summary>
    public ValidationResult ValidateValidationRules(Configuration configuration)
    {
        var result = new ValidationResult { IsValid = true, ValidatorName = nameof(ConfigurationValidator) };
        
        if (configuration.ValidationRules == null || configuration.ValidationRules.Count == 0)
        {
            return result;
        }
        
        foreach (var rule in configuration.ValidationRules)
        {
            if (string.IsNullOrWhiteSpace(rule.Key))
            {
                result.AddError("ValidationRules", "Validation rule key cannot be empty", ErrorCode.ValidationError);
                continue;
            }
            
            // Validate common rule types
            switch (rule.Key.ToLowerInvariant())
            {
                case "minvalue":
                case "maxvalue":
                    if (rule.Value == null || !decimal.TryParse(rule.Value.ToString(), out _))
                    {
                        result.AddError("ValidationRules", $"Rule '{rule.Key}' must have a numeric value", ErrorCode.ValidationError);
                    }
                    break;
                    
                case "minlength":
                case "maxlength":
                    if (rule.Value == null || !int.TryParse(rule.Value.ToString(), out _))
                    {
                        result.AddError("ValidationRules", $"Rule '{rule.Key}' must have an integer value", ErrorCode.ValidationError);
                    }
                    break;
                    
                case "pattern":
                    try
                    {
                        if (rule.Value != null)
                        {
                            _ = new System.Text.RegularExpressions.Regex(rule.Value.ToString()!);
                        }
                    }
                    catch
                    {
                        result.AddError("ValidationRules", "Pattern rule must be a valid regular expression", ErrorCode.ValidationError);
                    }
                    break;
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Validates configuration for update
    /// </summary>
    public ValidationResult ValidateForUpdate(Configuration existing, Configuration updated)
    {
        var result = Validate(updated);
        
        if (existing.IsReadOnly)
        {
            result.AddError("Configuration", "Cannot update read-only configuration", ErrorCode.ValidationError);
        }
        
        if (existing.Name != updated.Name)
        {
            result.AddWarning(nameof(updated.Name), "Configuration name has changed");
        }
        
        if (existing.Type != updated.Type)
        {
            result.AddWarning(nameof(updated.Type), "Configuration type has changed");
        }
        
        return result;
    }
}
