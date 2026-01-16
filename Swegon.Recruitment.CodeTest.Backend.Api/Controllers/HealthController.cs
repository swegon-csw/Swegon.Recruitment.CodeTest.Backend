using Microsoft.AspNetCore.Mvc;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController(ILogger<HealthController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
        logger.LogDebug("Health check requested");
        return Ok(
            new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
            }
        );
    }

    [HttpGet("ready")]
    public IActionResult GetReadiness()
    {
        return Ok(new { status = "Ready", timestamp = DateTime.UtcNow });
    }

    [HttpGet("live")]
    public IActionResult GetLiveness()
    {
        return Ok(new { status = "Alive", timestamp = DateTime.UtcNow });
    }
}
