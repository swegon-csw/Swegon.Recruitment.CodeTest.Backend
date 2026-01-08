using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Interfaces;

/// <summary>
/// Interface for calculation service operations
/// </summary>
public interface ICalculationService
{
    /// <summary>
    /// Performs a calculation based on the request
    /// </summary>
    Task<CalculationResponse> CalculateAsync(CalculationRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets calculation history for a product
    /// </summary>
    Task<PagedResponse<CalculationResponse>> GetCalculationHistoryAsync(Guid productId, FilterRequest filter, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a specific calculation by ID
    /// </summary>
    Task<CalculationResponse?> GetCalculationByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Performs batch calculations
    /// </summary>
    Task<BatchResponse<CalculationResponse>> BatchCalculateAsync(BatchRequest<CalculationRequest> request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates calculation parameters
    /// </summary>
    Task<ValidationResponse> ValidateCalculationAsync(CalculationRequest request, CancellationToken cancellationToken = default);
}
