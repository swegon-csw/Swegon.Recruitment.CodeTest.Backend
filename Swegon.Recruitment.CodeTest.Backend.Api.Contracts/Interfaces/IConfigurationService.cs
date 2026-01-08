using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Interfaces;

/// <summary>
/// Interface for configuration service operations
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Gets all configurations with optional filtering
    /// </summary>
    Task<PagedResponse<ConfigurationResponse>> GetConfigurationsAsync(FilterRequest filter, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a configuration by key
    /// </summary>
    Task<ConfigurationResponse?> GetConfigurationByKeyAsync(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates or updates a configuration
    /// </summary>
    Task<ConfigurationResponse> SetConfigurationAsync(ConfigurationRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a configuration
    /// </summary>
    Task<bool> DeleteConfigurationAsync(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets configurations by category
    /// </summary>
    Task<List<ConfigurationResponse>> GetConfigurationsByCategoryAsync(string category, CancellationToken cancellationToken = default);
}
