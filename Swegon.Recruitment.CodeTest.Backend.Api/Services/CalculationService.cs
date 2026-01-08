using Microsoft.Extensions.Logging;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Interfaces;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;
using Swegon.Recruitment.CodeTest.Backend.Api.Helpers;
using Swegon.Recruitment.CodeTest.Backend.Api.Calculators;
using Swegon.Recruitment.CodeTest.Backend.Api.Validators;
using System.Collections.Concurrent;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Services;

/// <summary>
/// Calculation service with comprehensive calculation management and caching
/// </summary>
public class CalculationService : ICalculationService
{
    private readonly ILogger<CalculationService> _logger;
    private readonly ProductService _productService;
    private readonly CacheService _cacheService;
    private readonly CalculationRequestValidator _validator;
    private readonly PrimaryCalculator _primaryCalculator;
    private readonly ComplexCalculator _complexCalculator;
    private readonly ConcurrentDictionary<Guid, Calculation> _calculations;
    
    public CalculationService(
        ILogger<CalculationService> logger,
        ProductService productService,
        CacheService cacheService,
        CalculationRequestValidator validator,
        PrimaryCalculator primaryCalculator,
        ComplexCalculator complexCalculator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _primaryCalculator = primaryCalculator ?? throw new ArgumentNullException(nameof(primaryCalculator));
        _complexCalculator = complexCalculator ?? throw new ArgumentNullException(nameof(complexCalculator));
        _calculations = new ConcurrentDictionary<Guid, Calculation>();
    }
    
    public async Task<CalculationResponse> CalculateAsync(
        CalculationRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting calculation for product {ProductId}", request.ProductId);
        
        var validation = _validator.Validate(request);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException($"Validation failed: {string.Join(", ", validation.Errors.Select(e => e.Message))}");
        }
        
        var productResponse = await _productService.GetProductByIdAsync(request.ProductId, cancellationToken);
        if (productResponse == null)
        {
            throw new InvalidOperationException($"Product {request.ProductId} not found");
        }
        
        var product = new Product
        {
            Id = productResponse.Id,
            Name = productResponse.Name,
            Price = productResponse.Price,
            Type = productResponse.Type
        };
        
        var calculator = request.Parameters.ContainsKey("useComplexCalculator") &&
                        Convert.ToBoolean(request.Parameters["useComplexCalculator"])
            ? _complexCalculator
            : _primaryCalculator;
        
        var calculationResult = await calculator.CalculateAsync(product, request.Quantity, request.Parameters, cancellationToken);
        
        var calculation = new Calculation
        {
            ProductId = request.ProductId,
            Status = calculationResult.Status,
            Total = calculationResult.Total,
            Subtotal = calculationResult.Subtotal,
            DiscountAmount = calculationResult.DiscountAmount,
            TaxAmount = calculationResult.TaxAmount,
            Quantity = request.Quantity,
            InputParameters = request.Parameters,
            Breakdown = calculationResult.Steps,
            Metadata = calculationResult.Metadata,
            ProcessingTimeMs = calculationResult.ProcessingDurationMs
        };
        
        _calculations.TryAdd(calculation.Id, calculation);
        
        _logger.LogInformation("Calculation {CalculationId} completed with total {Total}", 
            calculation.Id, calculation.Total);
        
        return MappingHelper.MapToResponse(calculation);
    }
    
    public async Task<PagedResponse<CalculationResponse>> GetCalculationHistoryAsync(
        Guid productId,
        FilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        var history = _calculations.Values
            .Where(c => c.ProductId == productId)
            .OrderByDescending(c => c.CalculatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        
        var totalCount = _calculations.Values.Count(c => c.ProductId == productId);
        var responses = MappingHelper.MapToResponseList(history);
        
        return MappingHelper.CreatePagedResponse(responses, filter.Page, filter.PageSize, totalCount);
    }
    
    public async Task<CalculationResponse?> GetCalculationByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (_calculations.TryGetValue(id, out var calculation))
        {
            return MappingHelper.MapToResponse(calculation);
        }
        
        return null;
    }
    
    public async Task<BatchResponse<CalculationResponse>> BatchCalculateAsync(
        BatchRequest<CalculationRequest> request,
        CancellationToken cancellationToken = default)
    {
        var responses = new List<CalculationResponse>();
        var errors = new List<string>();
        
        foreach (var item in request.Items)
        {
            try
            {
                var response = await CalculateAsync(item, cancellationToken);
                responses.Add(response);
            }
            catch (Exception ex)
            {
                errors.Add($"Error calculating for product {item.ProductId}: {ex.Message}");
            }
        }
        
        return new BatchResponse<CalculationResponse>
        {
            Items = responses,
            SuccessCount = responses.Count,
            FailureCount = errors.Count,
            Errors = errors
        };
    }
    
    public async Task<ValidationResponse> ValidateCalculationAsync(
        CalculationRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = _validator.Validate(request);
        return MappingHelper.MapToResponse(validation);
    }
}
