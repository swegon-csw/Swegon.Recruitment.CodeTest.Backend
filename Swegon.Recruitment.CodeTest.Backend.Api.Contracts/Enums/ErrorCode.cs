namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

/// <summary>
/// Error codes for API operations
/// </summary>
public enum ErrorCode
{
    /// <summary>
    /// Unknown error
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// Validation error
    /// </summary>
    ValidationError = 1000,
    
    /// <summary>
    /// Resource not found
    /// </summary>
    NotFound = 2000,
    
    /// <summary>
    /// Unauthorized access
    /// </summary>
    Unauthorized = 3000,
    
    /// <summary>
    /// Internal server error
    /// </summary>
    InternalError = 5000,
    
    /// <summary>
    /// External service error
    /// </summary>
    ExternalServiceError = 6000,
    
    /// <summary>
    /// Calculation error
    /// </summary>
    CalculationError = 7000
}
