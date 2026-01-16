using System.Text.Json;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Services;

public class JsonDataService(ILogger<JsonDataService> logger, IWebHostEnvironment env)
{
    private readonly string _dataDirectory = Path.Combine(env.ContentRootPath, "Data");
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    public async Task<T> LoadDataAsync<T>(string fileName)
        where T : new()
    {
        var filePath = Path.Combine(_dataDirectory, fileName);
        if (!File.Exists(filePath))
        {
            logger.LogWarning("File not found at {Path}", filePath);
            return new T();
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<T>(json, _jsonOptions) ?? new T();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading data from {FileName}", fileName);
            return new T();
        }
    }

    public async Task SaveDataAsync<T>(string fileName, T data)
    {
        var filePath = Path.Combine(_dataDirectory, fileName);
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving data to {FileName}", fileName);
            throw;
        }
    }
}
