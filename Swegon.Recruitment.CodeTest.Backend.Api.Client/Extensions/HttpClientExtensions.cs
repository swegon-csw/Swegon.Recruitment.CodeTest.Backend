using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Client.Extensions;

/// <summary>
/// Extension methods for HttpClient.
/// </summary>
public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Sends a GET request and deserializes the JSON response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response.</returns>
    public static async Task<T?> GetFromJsonAsync<T>(
        this HttpClient httpClient,
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<T>(requestUri, DefaultJsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a POST request with JSON content and deserializes the JSON response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <typeparam name="TResponse">The type to deserialize the response to.</typeparam>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response.</returns>
    public static async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        string requestUri,
        TRequest value,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(requestUri, value, DefaultJsonOptions, cancellationToken)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>(DefaultJsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a PUT request with JSON content and deserializes the JSON response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <typeparam name="TResponse">The type to deserialize the response to.</typeparam>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response.</returns>
    public static async Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        string requestUri,
        TRequest value,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync(requestUri, value, DefaultJsonOptions, cancellationToken)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>(DefaultJsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a PATCH request with JSON content and deserializes the JSON response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <typeparam name="TResponse">The type to deserialize the response to.</typeparam>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response.</returns>
    public static async Task<TResponse?> PatchAsJsonAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        string requestUri,
        TRequest value,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value, DefaultJsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Patch, requestUri)
        {
            Content = content
        };

        var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>(DefaultJsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a DELETE request and deserializes the JSON response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response.</returns>
    public static async Task<T?> DeleteFromJsonAsync<T>(
        this HttpClient httpClient,
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync(requestUri, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(DefaultJsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a DELETE request without expecting a response body.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task DeleteAsync(
        this HttpClient httpClient,
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync(requestUri, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Reads and deserializes JSON content from an HttpResponseMessage.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="response">The HTTP response message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized content.</returns>
    public static async Task<T?> ReadFromJsonAsync<T>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return await response.Content.ReadFromJsonAsync<T>(DefaultJsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }
}
