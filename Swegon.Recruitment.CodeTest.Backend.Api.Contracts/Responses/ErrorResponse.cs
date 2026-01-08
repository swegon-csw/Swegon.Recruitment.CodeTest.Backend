using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

/// <summary>
/// Response for error scenarios
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error code
    /// </summary>
    public ErrorCode Code { get; set; }
    
    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed error description
    /// </summary>
    public string? Details { get; set; }
    
    /// <summary>
    /// Timestamp when error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Request ID for tracking
    /// </summary>
    public string? RequestId { get; set; }
    
    /// <summary>
    /// Additional error data
    /// </summary>
    public Dictionary<string, object>? AdditionalData { get; set; }
    
    /// <summary>
    /// Inner errors if any
    /// </summary>
    public List<ErrorResponse>? InnerErrors { get; set; }
}
