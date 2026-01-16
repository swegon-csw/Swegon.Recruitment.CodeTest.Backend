using Microsoft.AspNetCore.Mvc;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts;
using Swegon.Recruitment.CodeTest.Backend.Api.Services;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Controllers;

[ApiController]
[Route("api/calculations")]
public class CalculationsController(
    CalculationService calculationService,
    ILogger<CalculationsController> logger
) : ControllerBase
{
    [HttpPost]
    public ActionResult<CalculationResultDto> Calculate([FromBody] CalculationInputDto input)
    {
        try
        {
            logger.LogInformation(
                "Calculating - Area: {Area}, Height: {Height}, ActivityLevel: {ActivityLevel}",
                input.Area,
                input.Height,
                input.ActivityLevel
            );

            if (input.Area <= 0 || input.Height <= 0)
            {
                return BadRequest(new { error = "Area and height must be greater than zero" });
            }

            var result = calculationService.Calculate(input);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error performing calculation");
            return StatusCode(500, new { error = "An error occurred while calculating" });
        }
    }

    [HttpPost("v2")]
    public ActionResult<CalculationResultDto> CalculateV2([FromBody] CalculationInputDto input)
    {
        try
        {
            logger.LogInformation(
                "Calculating V2 - Area: {Area}, Height: {Height}, ActivityLevel: {ActivityLevel}",
                input.Area,
                input.Height,
                input.ActivityLevel
            );

            if (input.Area <= 0 || input.Height <= 0)
            {
                return BadRequest(new { error = "Area and height must be greater than zero" });
            }

            var result = calculationService.CalculateAdvanced(input);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error performing V2 calculation");
            return StatusCode(500, new { error = "An error occurred while calculating" });
        }
    }
}

public class SaveCalculationRequest
{
    public CalculationInputDto Input { get; set; } = new();
    public CalculationResultDto Result { get; set; } = new();
    public string? Name { get; set; }
}
