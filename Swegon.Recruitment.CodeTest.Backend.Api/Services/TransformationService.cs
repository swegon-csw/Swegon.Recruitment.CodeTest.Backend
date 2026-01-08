using Microsoft.Extensions.Logging;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Services;

/// <summary>
/// Transformation service for data transformations
/// </summary>
public class TransformationService
{
    private readonly ILogger<TransformationService> _logger;
    
    public TransformationService(ILogger<TransformationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<TOutput> TransformAsync<TInput, TOutput>(
        TInput input,
        Func<TInput, TOutput> transformer)
        where TInput : class
        where TOutput : class
    {
        _logger.LogInformation("Transforming {InputType} to {OutputType}",
            typeof(TInput).Name, typeof(TOutput).Name);
        
        await Task.Delay(5);
        
        return transformer(input);
    }
    
    public async Task<List<TOutput>> TransformCollectionAsync<TInput, TOutput>(
        IEnumerable<TInput> inputs,
        Func<TInput, TOutput> transformer)
        where TInput : class
        where TOutput : class
    {
        _logger.LogInformation("Transforming {Count} items", inputs.Count());
        await Task.Delay(5);
        
        return inputs.Select(transformer).ToList();
    }
}
