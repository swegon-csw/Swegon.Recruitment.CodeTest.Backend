namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts;

public class CalculationHistoryDto
{
    public string Id { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
    public string? Name { get; set; }
    public CalculationInputDto Input { get; set; } = new();
    public CalculationResultDto Result { get; set; } = new();
}
