namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts;

public class CalculationInputDto
{
    public double Area { get; set; }
    public double Height { get; set; }
    public int Occupancy { get; set; }
    public string ActivityLevel { get; set; } = "medium";
    public double Temperature { get; set; }
    public double HumidityPercent { get; set; }
}
