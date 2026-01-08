using Microsoft.Extensions.Logging;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Services;

/// <summary>
/// Data service for generic data operations
/// </summary>
public class DataService
{
    private readonly ILogger<DataService> _logger;
    
    public DataService(ILogger<DataService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<DataModel> GetDataAsync(Guid id)
    {
        _logger.LogInformation("Getting data {Id}", id);
        await Task.Delay(10);
        
        return new DataModel { Id = id, Name = "Sample Data" };
    }
    
    public async Task<DataModel> CreateDataAsync(DataModel data)
    {
        _logger.LogInformation("Creating data");
        await Task.Delay(10);
        
        return data;
    }
}
