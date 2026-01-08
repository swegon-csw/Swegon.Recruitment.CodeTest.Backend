using Microsoft.AspNetCore.Mvc;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Controllers;

/// <summary>
/// Configuration management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly ILogger<ConfigurationController> _logger;
    
    public ConfigurationController(ILogger<ConfigurationController> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Gets configuration by name
    /// </summary>
    [HttpGet("{name}")]
    public IActionResult GetConfiguration(string name)
    {
        _logger.LogInformation("Getting configuration {Name}", name);
        
        var config = new Configuration
        {
            Name = name,
            Value = "sample-value",
            Category = "General"
        };
        
        return Ok(config);
    }
    
    /// <summary>
    /// Lists all configurations
    /// </summary>
    [HttpGet]
    public IActionResult ListConfigurations()
    {
        _logger.LogInformation("Listing configurations");
        return Ok(new List<Configuration>());
    }
}
