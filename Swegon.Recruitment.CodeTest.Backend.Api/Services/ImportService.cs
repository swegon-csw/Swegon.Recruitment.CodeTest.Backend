using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Services;

/// <summary>
/// Import service for importing data from various formats
/// </summary>
public class ImportService
{
    private readonly ILogger<ImportService> _logger;
    
    public ImportService(ILogger<ImportService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<List<T>> ImportFromJsonAsync<T>(string json) where T : class
    {
        _logger.LogInformation("Importing data from JSON");
        await Task.Delay(10);
        
        try
        {
            var data = JsonSerializer.Deserialize<List<T>>(json);
            return data ?? new List<T>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import JSON data");
            throw;
        }
    }
    
    public async Task<List<Dictionary<string, string>>> ImportFromCsvAsync(string csv)
    {
        _logger.LogInformation("Importing data from CSV");
        await Task.Delay(10);
        
        var result = new List<Dictionary<string, string>>();
        var lines = csv.Split('\n');
        
        if (lines.Length == 0) return result;
        
        var headers = lines[0].Split(',');
        
        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');
            var row = new Dictionary<string, string>();
            
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                row[headers[j]] = values[j];
            }
            
            result.Add(row);
        }
        
        return result;
    }
}
