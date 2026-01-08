using System;
using System.Collections.Generic;
using System.Net;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Client.Configuration;

/// <summary>
/// Configuration for retry policies.
/// </summary>
public class RetryPolicyConfig
{
    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets or sets the initial delay between retries in milliseconds.
    /// </summary>
    public int InitialDelayMilliseconds { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the maximum delay between retries in milliseconds.
    /// </summary>
    public int MaxDelayMilliseconds { get; set; } = 30000;

    /// <summary>
    /// Gets or sets the backoff multiplier for exponential backoff.
    /// </summary>
    public double BackoffMultiplier { get; set; } = 2.0;

    /// <summary>
    /// Gets or sets a value indicating whether to use exponential backoff.
    /// </summary>
    public bool UseExponentialBackoff { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to add jitter to retry delays.
    /// </summary>
    public bool UseJitter { get; set; } = true;

    /// <summary>
    /// Gets or sets the jitter factor (0.0 to 1.0).
    /// </summary>
    public double JitterFactor { get; set; } = 0.1;

    /// <summary>
    /// Gets or sets the HTTP status codes that should trigger a retry.
    /// </summary>
    public HashSet<HttpStatusCode> RetryableStatusCodes { get; set; } = new()
    {
        HttpStatusCode.RequestTimeout,
        HttpStatusCode.TooManyRequests,
        HttpStatusCode.InternalServerError,
        HttpStatusCode.BadGateway,
        HttpStatusCode.ServiceUnavailable,
        HttpStatusCode.GatewayTimeout
    };

    /// <summary>
    /// Gets the initial delay as a TimeSpan.
    /// </summary>
    public TimeSpan InitialDelay => TimeSpan.FromMilliseconds(InitialDelayMilliseconds);

    /// <summary>
    /// Gets the maximum delay as a TimeSpan.
    /// </summary>
    public TimeSpan MaxDelay => TimeSpan.FromMilliseconds(MaxDelayMilliseconds);

    /// <summary>
    /// Calculates the delay for a specific retry attempt.
    /// </summary>
    /// <param name="attemptNumber">The retry attempt number (starting from 1).</param>
    /// <returns>The delay duration.</returns>
    public TimeSpan CalculateDelay(int attemptNumber)
    {
        if (attemptNumber <= 0)
        {
            throw new ArgumentException("Attempt number must be positive.", nameof(attemptNumber));
        }

        double delayMs;

        if (UseExponentialBackoff)
        {
            // Exponential backoff: initialDelay * (multiplier ^ (attempt - 1))
            delayMs = InitialDelayMilliseconds * Math.Pow(BackoffMultiplier, attemptNumber - 1);
        }
        else
        {
            // Linear backoff
            delayMs = InitialDelayMilliseconds * attemptNumber;
        }

        // Cap at maximum delay
        delayMs = Math.Min(delayMs, MaxDelayMilliseconds);

        // Add jitter if enabled
        if (UseJitter)
        {
            var random = new Random();
            var jitterRange = delayMs * JitterFactor;
            var jitter = (random.NextDouble() * 2 - 1) * jitterRange; // Random value between -jitterRange and +jitterRange
            delayMs += jitter;
            delayMs = Math.Max(0, delayMs); // Ensure non-negative
        }

        return TimeSpan.FromMilliseconds(delayMs);
    }

    /// <summary>
    /// Determines whether a status code should trigger a retry.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <returns>True if the request should be retried; otherwise, false.</returns>
    public bool ShouldRetry(HttpStatusCode statusCode)
    {
        return RetryableStatusCodes.Contains(statusCode);
    }

    /// <summary>
    /// Validates the configuration.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when configuration is invalid.</exception>
    public void Validate()
    {
        if (MaxRetries < 0)
        {
            throw new InvalidOperationException("MaxRetries cannot be negative.");
        }

        if (InitialDelayMilliseconds <= 0)
        {
            throw new InvalidOperationException("InitialDelayMilliseconds must be greater than 0.");
        }

        if (MaxDelayMilliseconds <= 0)
        {
            throw new InvalidOperationException("MaxDelayMilliseconds must be greater than 0.");
        }

        if (InitialDelayMilliseconds > MaxDelayMilliseconds)
        {
            throw new InvalidOperationException("InitialDelayMilliseconds cannot be greater than MaxDelayMilliseconds.");
        }

        if (BackoffMultiplier <= 1.0)
        {
            throw new InvalidOperationException("BackoffMultiplier must be greater than 1.0.");
        }

        if (JitterFactor < 0.0 || JitterFactor > 1.0)
        {
            throw new InvalidOperationException("JitterFactor must be between 0.0 and 1.0.");
        }
    }

    /// <summary>
    /// Creates a default configuration.
    /// </summary>
    /// <returns>A new <see cref="RetryPolicyConfig"/> instance with default values.</returns>
    public static RetryPolicyConfig Default() => new();

    /// <summary>
    /// Creates a configuration with no retries.
    /// </summary>
    /// <returns>A new <see cref="RetryPolicyConfig"/> instance with retries disabled.</returns>
    public static RetryPolicyConfig NoRetry() => new() { MaxRetries = 0 };

    /// <summary>
    /// Creates an aggressive retry configuration for development/testing.
    /// </summary>
    /// <returns>A new <see cref="RetryPolicyConfig"/> instance with aggressive retry settings.</returns>
    public static RetryPolicyConfig Aggressive() => new()
    {
        MaxRetries = 5,
        InitialDelayMilliseconds = 500,
        MaxDelayMilliseconds = 10000,
        BackoffMultiplier = 1.5
    };
}
