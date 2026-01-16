using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

public class ErrorResponse
{
    public ErrorCode Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? RequestId { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
    public List<ErrorResponse>? InnerErrors { get; set; }
}
