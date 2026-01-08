using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Swegon.Recruitment.CodeTest.Backend.Api.Client.Configuration;
using Swegon.Recruitment.CodeTest.Backend.Api.Client.Exceptions;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Client.Clients;

/// <summary>
/// Abstract base class for API clients providing common HTTP functionality.
/// </summary>
public abstract class BaseClient
{
    /// <summary>
    /// Gets the HTTP client.
    /// </summary>
    protected HttpClient HttpClient { get; }

    /// <summary>
    /// Gets the client configuration.
    /// </summary>
    protected ClientConfiguration Configuration { get; }

    /// <summary>
    /// Gets the JSON serializer options.
    /// </summary>
    protected JsonSerializerOptions JsonOptions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="configuration">The client configuration.</param>
    protected BaseClient(HttpClient httpClient, ClientConfiguration configuration)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Sends a GET request and returns the deserialized response.
    /// </summary>
    /// <typeparam name="T">The response type.</typeparam>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response.</returns>
    protected async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => HttpClient.GetAsync(endpoint, cancellationToken),
            endpoint,
            cancellationToken).ConfigureAwait(false);

        return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a POST request and returns the deserialized response.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="request">The request body.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response.</returns>
    protected async Task<TResponse> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => HttpClient.PostAsJsonAsync(endpoint, request, JsonOptions, cancellationToken),
            endpoint,
            cancellationToken).ConfigureAwait(false);

        return await DeserializeResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a PUT request and returns the deserialized response.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="request">The request body.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response.</returns>
    protected async Task<TResponse> PutAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => HttpClient.PutAsJsonAsync(endpoint, request, JsonOptions, cancellationToken),
            endpoint,
            cancellationToken).ConfigureAwait(false);

        return await DeserializeResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a DELETE request.
    /// </summary>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        await SendRequestAsync(
            () => HttpClient.DeleteAsync(endpoint, cancellationToken),
            endpoint,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a DELETE request and returns the deserialized response.
    /// </summary>
    /// <typeparam name="T">The response type.</typeparam>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response.</returns>
    protected async Task<T> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => HttpClient.DeleteAsync(endpoint, cancellationToken),
            endpoint,
            cancellationToken).ConfigureAwait(false);

        return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP request with retry logic.
    /// </summary>
    private async Task<HttpResponseMessage> SendRequestAsync(
        Func<Task<HttpResponseMessage>> requestFunc,
        string endpoint,
        CancellationToken cancellationToken)
    {
        var retryConfig = new RetryPolicyConfig { MaxRetries = Configuration.MaxRetries };
        Exception? lastException = null;

        for (int attempt = 0; attempt <= retryConfig.MaxRetries; attempt++)
        {
            try
            {
                var response = await requestFunc().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

                if (attempt < retryConfig.MaxRetries && retryConfig.ShouldRetry(response.StatusCode))
                {
                    var delay = retryConfig.CalculateDelay(attempt + 1);
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                await HandleErrorResponseAsync(response, endpoint, cancellationToken).ConfigureAwait(false);
                return response;
            }
            catch (HttpRequestException ex) when (attempt < retryConfig.MaxRetries)
            {
                lastException = ex;
                var delay = retryConfig.CalculateDelay(attempt + 1);
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                throw new ConnectionException(
                    $"Request to {endpoint} timed out after {Configuration.TimeoutSeconds} seconds.",
                    endpoint,
                    ex);
            }
        }

        throw new ConnectionException(
            $"Failed to connect to {endpoint} after {retryConfig.MaxRetries} retry attempts.",
            endpoint,
            retryConfig.MaxRetries,
            lastException!);
    }

    /// <summary>
    /// Handles error responses by throwing appropriate exceptions.
    /// </summary>
    private async Task HandleErrorResponseAsync(
        HttpResponseMessage response,
        string endpoint,
        CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            // Try to parse validation errors
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ValidationErrorResponse>(content, JsonOptions);
                if (errorResponse?.Errors != null && errorResponse.Errors.Count > 0)
                {
                    throw new ValidationException(
                        errorResponse.Title ?? "Validation failed.",
                        errorResponse.Errors,
                        content,
                        endpoint);
                }
            }
            catch (JsonException)
            {
                // Not a validation error response, fall through to generic error
            }
        }

        var message = $"API request failed with status {(int)response.StatusCode} ({response.StatusCode}).";

        throw new ApiClientException(
            message,
            response.StatusCode,
            content,
            endpoint);
    }

    /// <summary>
    /// Deserializes an HTTP response to the specified type.
    /// </summary>
    private async Task<T> DeserializeResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken)
                .ConfigureAwait(false);

            if (result == null)
            {
                throw new ApiClientException(
                    "Response content was null or could not be deserialized.",
                    response.StatusCode,
                    await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false),
                    response.RequestMessage?.RequestUri?.ToString());
            }

            return result;
        }
        catch (JsonException ex)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new ApiClientException(
                $"Failed to deserialize response: {ex.Message}",
                response.StatusCode,
                content,
                response.RequestMessage?.RequestUri?.ToString(),
                ex);
        }
    }

    /// <summary>
    /// Internal class for parsing validation error responses.
    /// </summary>
    private class ValidationErrorResponse
    {
        public string? Title { get; set; }
        public System.Collections.Generic.Dictionary<string, string[]>? Errors { get; set; }
    }
}
