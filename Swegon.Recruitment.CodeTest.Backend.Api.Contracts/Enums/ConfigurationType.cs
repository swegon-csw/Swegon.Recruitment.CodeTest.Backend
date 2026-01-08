namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

/// <summary>
/// Type of configuration
/// </summary>
public enum ConfigurationType
{
    /// <summary>
    /// Global configuration
    /// </summary>
    Global = 0,
    
    /// <summary>
    /// User-specific configuration
    /// </summary>
    User = 1,
    
    /// <summary>
    /// Product-specific configuration
    /// </summary>
    Product = 2,
    
    /// <summary>
    /// System configuration
    /// </summary>
    System = 3
}
