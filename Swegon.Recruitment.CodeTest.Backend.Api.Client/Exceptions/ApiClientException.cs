using System;
using System.Net;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Client.Exceptions;

/// <summary>
/// Base exception class for all API client errors.
/// </summary>
public class ApiClientException : Exception
{
    /// <summary>
    /// Gets the HTTP status code of the response that caused the exception.
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// Gets the raw response content.
    /// </summary>
    public string? ResponseContent { get; }

    /// <summary>
    /// Gets the request URI that caused the exception.
    /// </summary>
    public string? RequestUri { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClientException"/> class.
    /// </summary>
    public ApiClientException()
        : base("An error occurred while communicating with the API.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClientException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ApiClientException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClientException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ApiClientException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClientException"/> class with HTTP details.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="responseContent">The response content.</param>
    /// <param name="requestUri">The request URI.</param>
    public ApiClientException(
        string message,
        HttpStatusCode statusCode,
        string? responseContent = null,
        string? requestUri = null)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
        RequestUri = requestUri;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClientException"/> class with HTTP details and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="responseContent">The response content.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="innerException">The inner exception.</param>
    public ApiClientException(
        string message,
        HttpStatusCode statusCode,
        string? responseContent,
        string? requestUri,
        Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
        RequestUri = requestUri;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var baseMessage = base.ToString();
        
        if (StatusCode.HasValue)
        {
            baseMessage += $"\nStatus Code: {(int)StatusCode.Value} ({StatusCode.Value})";
        }

        if (!string.IsNullOrEmpty(RequestUri))
        {
            baseMessage += $"\nRequest URI: {RequestUri}";
        }

        if (!string.IsNullOrEmpty(ResponseContent))
        {
            var contentPreview = ResponseContent.Length > 500 
                ? ResponseContent.Substring(0, 500) + "..." 
                : ResponseContent;
            baseMessage += $"\nResponse Content: {contentPreview}";
        }

        return baseMessage;
    }
}
