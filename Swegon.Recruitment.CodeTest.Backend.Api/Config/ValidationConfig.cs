namespace Swegon.Recruitment.CodeTest.Backend.Api.Config;

/// <summary>
/// Validation configuration settings
/// </summary>
public class ValidationConfig
{
    /// <summary>
    /// Whether to enable strict validation
    /// </summary>
    public bool StrictMode { get; set; } = true;
    
    /// <summary>
    /// Maximum validation depth for nested objects
    /// </summary>
    public int MaxDepth { get; set; } = 10;
    
    /// <summary>
    /// Custom validation rules
    /// </summary>
    public Dictionary<string, string> CustomRules { get; set; } = new();
    
    /// <summary>
    /// Whether to include validation errors in response
    /// </summary>
    public bool IncludeErrorDetails { get; set; } = true;
}
