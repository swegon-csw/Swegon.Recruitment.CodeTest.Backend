namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

public class CalculationStatisticsResponse
{
    public int TotalCalculations { get; set; }
    public decimal AverageResult { get; set; }
    public decimal MinResult { get; set; }
    public decimal MaxResult { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
