namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

public class HealthResponse
{
    public string Status { get; set; } = "Healthy";
    public Dictionary<string, ComponentHealth> Components { get; set; } = new();
    public string Version { get; set; } = "1.0.0";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public TimeSpan Uptime { get; set; }
    public string? Message { get; set; }
}

public class ComponentHealth
{
    public string Status { get; set; } = "Healthy";
    public string? Description { get; set; }
    public long? ResponseTimeMs { get; set; }
    public Dictionary<string, object>? Data { get; set; }
}
