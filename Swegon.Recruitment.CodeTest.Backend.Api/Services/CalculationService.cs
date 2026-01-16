using Swegon.Recruitment.CodeTest.Backend.Api.Contracts;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Services;

public class CalculationService()
{
    private List<CalculationHistoryDto>? _calculationsCache;

    public CalculationResultDto Calculate(CalculationInputDto input)
    {
        var volume = input.Area * input.Height;
        var activityMultiplier = input.ActivityLevel.ToLower() switch
        {
            "low" => 2.0,
            "high" => 4.0,
            _ => 3.0,
        };

        var airflow = Math.Round(volume * 6 * activityMultiplier / 3600, 2);
        var coolingCapacity = (int)Math.Round(input.Area * 100 * activityMultiplier);
        var heatingCapacity = (int)Math.Round(input.Area * 80 * activityMultiplier);
        var energyConsumption = Math.Round((coolingCapacity + heatingCapacity) * 0.7 / 1000, 2);

        return new CalculationResultDto
        {
            Airflow = airflow,
            CoolingCapacity = coolingCapacity,
            HeatingCapacity = heatingCapacity,
            EnergyConsumption = energyConsumption,
            Cost = new CalculationCost
            {
                Installation = (int)Math.Round(input.Area * 50),
                Annual = (int)Math.Round(energyConsumption * 365 * 0.15),
                Lifetime = (int)Math.Round(energyConsumption * 365 * 15 * 0.15),
            },
            Recommendations = new List<string>
            {
                "Consider high-efficiency equipment for energy savings",
                "Regular maintenance recommended every 6 months",
            },
        };
    }

    // TODO: Needs to be refactored
    public CalculationResultDto? CalculateAdvanced(CalculationInputDto input)
    {
        var result = new CalculationResultDto
        {
            Airflow = 0,
            CoolingCapacity = 0,
            HeatingCapacity = 0,
            EnergyConsumption = 0,
            Cost = new CalculationCost(),
            Recommendations = new List<string>(),
        };

        if (input != null)
        {
            if (input.Area > 0)
            {
                if (input.Height > 0)
                {
                    var volume = input.Area * input.Height;
                    if (volume > 0)
                    {
                        var activityMultiplier = 1.5;

                        if (input.ActivityLevel != null)
                        {
                            if (input.ActivityLevel.ToLower() == "low")
                            {
                                activityMultiplier = 1.0;
                            }
                            else if (input.ActivityLevel.ToLower() == "high")
                            {
                                activityMultiplier = 2.0;
                            }

                            if (input.Temperature > 0)
                            {
                                var baseAirflow = volume * 6 * activityMultiplier / 3600;
                                result.Airflow = Math.Round(baseAirflow, 2);
                                result.AirflowPerHumidityPercent = Math.Round(
                                    baseAirflow / input.HumidityPercent,
                                    4
                                );
                                result.CoolingCapacity = (int)
                                    Math.Round(input.Area * 100 * activityMultiplier);
                                result.HeatingCapacity = (int)
                                    Math.Round(input.Area * 80 * activityMultiplier);
                                result.EnergyConsumption = Math.Round(
                                    (result.CoolingCapacity + result.HeatingCapacity) * 0.7 / 1000,
                                    2
                                );
                                result.Cost.Installation = (int)Math.Round(input.Area * 50);
                                result.Cost.Annual = (int)
                                    Math.Round(result.EnergyConsumption * 365 * 0.15);
                                result.Cost.Lifetime = (int)
                                    Math.Round(result.EnergyConsumption * 365 * 15 * 0.15);

                                if (input.Temperature > 25)
                                {
                                    if (input.HumidityPercent > 60)
                                    {
                                        result.Recommendations.Add(
                                            "High temperature and humidity detected"
                                        );
                                    }
                                }

                                return result;
                            }
                        }
                    }
                }
            }
        }

        return null;
    }
}
