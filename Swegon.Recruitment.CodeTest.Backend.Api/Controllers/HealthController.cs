using Microsoft.AspNetCore.Mvc;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Controllers;

/// <summary>
/// Health check controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;
    
    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), 200)]
    public IActionResult GetHealth()
    {
        _logger.LogDebug("Health check requested");
        
        return Ok(new HealthResponse
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
        });
    }
    
    /// <summary>
    /// Readiness check endpoint
    /// </summary>
    [HttpGet("ready")]
    public IActionResult GetReadiness()
    {
        return Ok(new { status = "Ready", timestamp = DateTime.UtcNow });
    }
    
    /// <summary>
    /// Liveness check endpoint
    /// </summary>
    [HttpGet("live")]
    public IActionResult GetLiveness()
    {
        return Ok(new { status = "Alive", timestamp = DateTime.UtcNow });
    }
}
