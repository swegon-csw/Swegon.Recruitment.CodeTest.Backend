using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Swegon.Recruitment.CodeTest.Backend.Api.Client.Configuration;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Client.Clients;

/// <summary>
/// Interface for calculation client operations.
/// </summary>
public interface ICalculationClient
{
    Task<CalculationResponse> CalculateAsync(CalculationRequest request, CancellationToken cancellationToken = default);
    Task<PagedResponse<CalculationResponse>> GetCalculationHistoryAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<BatchResponse<CalculationResponse>> BatchCalculateAsync(BatchRequest<CalculationRequest> request, CancellationToken cancellationToken = default);
    Task<CalculationResponse> GetCalculationByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Client for interacting with the Calculations API.
/// </summary>
public class CalculationClient : BaseClient, ICalculationClient
{
    private const string BaseEndpoint = "api/calculations";

    /// <summary>
    /// Initializes a new instance of the <see cref="CalculationClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="configuration">The client configuration.</param>
    public CalculationClient(HttpClient httpClient, ClientConfiguration configuration)
        : base(httpClient, configuration)
    {
    }

    /// <summary>
    /// Performs a calculation based on the provided request.
    /// </summary>
    /// <param name="request">The calculation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The calculation result.</returns>
    public async Task<CalculationResponse> CalculateAsync(
        CalculationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return await PostAsync<CalculationRequest, CalculationResponse>(
            BaseEndpoint,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the calculation history with pagination.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of calculation history.</returns>
    public async Task<PagedResponse<CalculationResponse>> GetCalculationHistoryAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0.");
        }

        if (pageSize < 1 || pageSize > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be between 1 and 100.");
        }

        var endpoint = $"{BaseEndpoint}/history?page={page}&pageSize={pageSize}";
        return await GetAsync<PagedResponse<CalculationResponse>>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs batch calculations for multiple requests.
    /// </summary>
    /// <param name="request">The batch calculation request containing multiple calculations.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The batch calculation results.</returns>
    public async Task<BatchResponse<CalculationResponse>> BatchCalculateAsync(
        BatchRequest<CalculationRequest> request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.Items == null || request.Items.Count == 0)
        {
            throw new ArgumentException("Batch request must contain at least one item.", nameof(request));
        }

        var endpoint = $"{BaseEndpoint}/batch";
        return await PostAsync<BatchRequest<CalculationRequest>, BatchResponse<CalculationResponse>>(
            endpoint,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a specific calculation by its unique identifier.
    /// </summary>
    /// <param name="id">The calculation ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The calculation details.</returns>
    public async Task<CalculationResponse> GetCalculationByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Calculation ID cannot be empty.", nameof(id));
        }

        var endpoint = $"{BaseEndpoint}/{id}";
        return await GetAsync<CalculationResponse>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Validates a calculation request without persisting it.
    /// </summary>
    /// <param name="request">The calculation request to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The validation result.</returns>
    public async Task<ValidationResponse> ValidateCalculationAsync(
        CalculationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var endpoint = $"{BaseEndpoint}/validate";
        return await PostAsync<CalculationRequest, ValidationResponse>(
            endpoint,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets calculation statistics for a given time period.
    /// </summary>
    /// <param name="startDate">The start date for statistics.</param>
    /// <param name="endDate">The end date for statistics.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Calculation statistics.</returns>
    public async Task<CalculationStatisticsResponse> GetCalculationStatisticsAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        if (endDate < startDate)
        {
            throw new ArgumentException("End date must be after start date.");
        }

        var endpoint = $"{BaseEndpoint}/statistics?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        return await GetAsync<CalculationStatisticsResponse>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a calculation from history.
    /// </summary>
    /// <param name="id">The calculation ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task DeleteCalculationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Calculation ID cannot be empty.", nameof(id));
        }

        var endpoint = $"{BaseEndpoint}/{id}";
        await DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Exports calculation history to a file format.
    /// </summary>
    /// <param name="request">The export request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The export response with download link.</returns>
    public async Task<ExportResponse> ExportCalculationsAsync(
        ExportRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var endpoint = $"{BaseEndpoint}/export";
        return await PostAsync<ExportRequest, ExportResponse>(
            endpoint,
            request,
            cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// Response model for calculation statistics (internal use).
/// </summary>
internal class CalculationStatisticsResponse
{
    public int TotalCalculations { get; set; }
    public decimal AverageValue { get; set; }
    public decimal MinValue { get; set; }
    public decimal MaxValue { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
