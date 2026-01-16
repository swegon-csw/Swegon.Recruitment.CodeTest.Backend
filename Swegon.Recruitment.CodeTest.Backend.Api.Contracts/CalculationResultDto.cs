namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts;

public class CalculationResultDto
{
    public double Airflow { get; set; }
    public double AirflowPerHumidityPercent { get; set; }
    public int CoolingCapacity { get; set; }
    public int HeatingCapacity { get; set; }
    public double EnergyConsumption { get; set; }
    public CalculationCost Cost { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}
