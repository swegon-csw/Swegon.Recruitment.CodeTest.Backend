using Microsoft.AspNetCore.Mvc;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;
using Swegon.Recruitment.CodeTest.Backend.Api.Services;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Controllers;

/// <summary>
/// Calculations REST API controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CalculationsController : ControllerBase
{
    private readonly ILogger<CalculationsController> _logger;
    private readonly CalculationService _calculationService;
    
    public CalculationsController(
        ILogger<CalculationsController> logger,
        CalculationService calculationService)
    {
        _logger = logger;
        _calculationService = calculationService;
    }
    
    /// <summary>
    /// Performs a calculation
    /// </summary>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(CalculationResponse), 200)]
    public async Task<IActionResult> Calculate(
        [FromBody] CalculationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _calculationService.CalculateAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    /// <summary>
    /// Gets calculation history for a product
    /// </summary>
    [HttpGet("history/{productId}")]
    [ProducesResponseType(typeof(PagedResponse<CalculationResponse>), 200)]
    public async Task<IActionResult> GetHistory(
        Guid productId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = new FilterRequest { Page = page, PageSize = pageSize };
            var result = await _calculationService.GetCalculationHistoryAsync(
                productId, filter, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting calculation history");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    /// <summary>
    /// Gets a calculation by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CalculationResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCalculation(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _calculationService.GetCalculationByIdAsync(id, cancellationToken);
            
            if (result == null)
            {
                return NotFound(new { error = $"Calculation {id} not found" });
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting calculation");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    /// <summary>
    /// Performs batch calculations
    /// </summary>
    [HttpPost("batch")]
    [ProducesResponseType(typeof(BatchResponse<CalculationResponse>), 200)]
    public async Task<IActionResult> BatchCalculate(
        [FromBody] BatchRequest<CalculationRequest> request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _calculationService.BatchCalculateAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error batch calculating");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}
