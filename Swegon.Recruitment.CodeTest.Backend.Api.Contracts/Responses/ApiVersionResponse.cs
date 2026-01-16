namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

public class ApiVersionResponse
{
    public string Version { get; set; } = "1.0.0";

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? BuildDate { get; set; }
}
