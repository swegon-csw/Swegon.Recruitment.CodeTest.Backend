using Microsoft.AspNetCore.Mvc;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Controllers;

[ApiController]
[Route("api/configuration")]
public class ConfigurationController(ILogger<ConfigurationController> logger) : ControllerBase
{
    [HttpGet("{name}")]
    public IActionResult GetConfiguration(string name)
    {
        logger.LogInformation("Getting configuration {Name}", name);
        return Ok(
            new
            {
                name,
                value = "sample-value",
                category = "General",
            }
        );
    }

    [HttpGet]
    public IActionResult ListConfigurations()
    {
        logger.LogInformation("Listing configurations");
        return Ok(new List<object>());
    }
}
