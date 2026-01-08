using Microsoft.Extensions.Logging;
using System.Text;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Services;

/// <summary>
/// Export service for exporting data to various formats
/// </summary>
public class ExportService
{
    private readonly ILogger<ExportService> _logger;
    
    public ExportService(ILogger<ExportService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<string> ExportToCsvAsync<T>(IEnumerable<T> data) where T : class
    {
        _logger.LogInformation("Exporting {Count} items to CSV", data.Count());
        await Task.Delay(10);
        
        var sb = new StringBuilder();
        var properties = typeof(T).GetProperties();
        
        sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));
        
        foreach (var item in data)
        {
            var values = properties.Select(p => p.GetValue(item)?.ToString() ?? "");
            sb.AppendLine(string.Join(",", values));
        }
        
        return sb.ToString();
    }
    
    public async Task<string> ExportToJsonAsync<T>(IEnumerable<T> data) where T : class
    {
        _logger.LogInformation("Exporting {Count} items to JSON", data.Count());
        await Task.Delay(10);
        
        return System.Text.Json.JsonSerializer.Serialize(data);
    }
}
