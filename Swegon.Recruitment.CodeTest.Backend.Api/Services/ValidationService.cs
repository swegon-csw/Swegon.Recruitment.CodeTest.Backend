using Microsoft.Extensions.Logging;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Validators;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Services;

/// <summary>
/// Validation service for cross-entity validation
/// </summary>
public class ValidationService
{
    private readonly ILogger<ValidationService> _logger;
    private readonly ProductValidator _productValidator;
    private readonly CalculationRequestValidator _calculationValidator;
    
    public ValidationService(
        ILogger<ValidationService> logger,
        ProductValidator productValidator,
        CalculationRequestValidator calculationValidator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _productValidator = productValidator ?? throw new ArgumentNullException(nameof(productValidator));
        _calculationValidator = calculationValidator ?? throw new ArgumentNullException(nameof(calculationValidator));
    }
    
    public async Task<ValidationResult> ValidateProductAsync(Product product)
    {
        _logger.LogInformation("Validating product {ProductId}", product.Id);
        await Task.Delay(5);
        
        return _productValidator.Validate(product);
    }
    
    public async Task<ValidationResult> ValidateAsync<T>(T entity) where T : class
    {
        _logger.LogInformation("Validating entity of type {Type}", typeof(T).Name);
        await Task.Delay(5);
        
        return new ValidationResult { IsValid = true };
    }
}
