using System;
using System.Net;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Client.Exceptions;

/// <summary>
/// Exception thrown when a connection to the API cannot be established.
/// </summary>
public class ConnectionException : ApiClientException
{
    /// <summary>
    /// Gets the number of retry attempts made.
    /// </summary>
    public int RetryCount { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ConnectionException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ConnectionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="innerException">The inner exception.</param>
    public ConnectionException(
        string message,
        string requestUri,
        Exception innerException)
        : base(message, innerException)
    {
        RequestUri = requestUri;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="retryCount">The number of retry attempts made.</param>
    /// <param name="innerException">The inner exception.</param>
    public ConnectionException(
        string message,
        string requestUri,
        int retryCount,
        Exception innerException)
        : base(message, innerException)
    {
        RequestUri = requestUri;
        RetryCount = retryCount;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var baseMessage = base.ToString();

        if (RetryCount > 0)
        {
            baseMessage += $"\nRetry Attempts: {RetryCount}";
        }

        return baseMessage;
    }
}
