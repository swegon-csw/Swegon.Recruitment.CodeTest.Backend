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
/// Interface for configuration client operations.
/// </summary>
public interface IConfigurationClient
{
    Task<IReadOnlyList<ConfigurationResponse>> GetConfigurationsAsync(CancellationToken cancellationToken = default);
    Task<ConfigurationResponse> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<ConfigurationResponse> SetConfigurationAsync(ConfigurationRequest request, CancellationToken cancellationToken = default);
    Task DeleteConfigurationAsync(string key, CancellationToken cancellationToken = default);
}

/// <summary>
/// Client for interacting with the Configuration API.
/// </summary>
public class ConfigurationClient : BaseClient, IConfigurationClient
{
    private const string BaseEndpoint = "api/configuration";

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="configuration">The client configuration.</param>
    public ConfigurationClient(HttpClient httpClient, ClientConfiguration configuration)
        : base(httpClient, configuration)
    {
    }

    /// <summary>
    /// Gets all configuration settings.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of all configuration settings.</returns>
    public async Task<IReadOnlyList<ConfigurationResponse>> GetConfigurationsAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<ConfigurationResponse>>(BaseEndpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a configuration setting by its key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The configuration setting.</returns>
    public async Task<ConfigurationResponse> GetByKeyAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Configuration key cannot be null or empty.", nameof(key));
        }

        var endpoint = $"{BaseEndpoint}/{Uri.EscapeDataString(key)}";
        return await GetAsync<ConfigurationResponse>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates or updates a configuration setting.
    /// </summary>
    /// <param name="request">The configuration request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created or updated configuration setting.</returns>
    public async Task<ConfigurationResponse> SetConfigurationAsync(
        ConfigurationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return await PostAsync<ConfigurationRequest, ConfigurationResponse>(
            BaseEndpoint,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a configuration setting.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task DeleteConfigurationAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Configuration key cannot be null or empty.", nameof(key));
        }

        var endpoint = $"{BaseEndpoint}/{Uri.EscapeDataString(key)}";
        await DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing configuration setting.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <param name="request">The configuration request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated configuration setting.</returns>
    public async Task<ConfigurationResponse> UpdateConfigurationAsync(
        string key,
        ConfigurationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Configuration key cannot be null or empty.", nameof(key));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var endpoint = $"{BaseEndpoint}/{Uri.EscapeDataString(key)}";
        return await PutAsync<ConfigurationRequest, ConfigurationResponse>(
            endpoint,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets configuration settings by category.
    /// </summary>
    /// <param name="category">The configuration category.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of configuration settings in the category.</returns>
    public async Task<IReadOnlyList<ConfigurationResponse>> GetByCategoryAsync(
        string category,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("Category cannot be null or empty.", nameof(category));
        }

        var endpoint = $"{BaseEndpoint}/category/{Uri.EscapeDataString(category)}";
        return await GetAsync<List<ConfigurationResponse>>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Searches configuration settings by name or value.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of matching configuration settings.</returns>
    public async Task<IReadOnlyList<ConfigurationResponse>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            throw new ArgumentException("Search term cannot be null or empty.", nameof(searchTerm));
        }

        var endpoint = $"{BaseEndpoint}/search?q={Uri.EscapeDataString(searchTerm)}";
        return await GetAsync<List<ConfigurationResponse>>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Bulk updates multiple configuration settings.
    /// </summary>
    /// <param name="requests">The list of configuration requests.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The list of updated configuration settings.</returns>
    public async Task<IReadOnlyList<ConfigurationResponse>> BulkUpdateAsync(
        IEnumerable<ConfigurationRequest> requests,
        CancellationToken cancellationToken = default)
    {
        if (requests == null)
        {
            throw new ArgumentNullException(nameof(requests));
        }

        var endpoint = $"{BaseEndpoint}/bulk";
        return await PostAsync<IEnumerable<ConfigurationRequest>, List<ConfigurationResponse>>(
            endpoint,
            requests,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Validates a configuration request without persisting it.
    /// </summary>
    /// <param name="request">The configuration request to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The validation result.</returns>
    public async Task<ValidationResponse> ValidateConfigurationAsync(
        ConfigurationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var endpoint = $"{BaseEndpoint}/validate";
        return await PostAsync<ConfigurationRequest, ValidationResponse>(
            endpoint,
            request,
            cancellationToken).ConfigureAwait(false);
    }
}
