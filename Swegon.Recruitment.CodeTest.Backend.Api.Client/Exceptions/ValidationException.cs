using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Client.Exceptions;

/// <summary>
/// Exception thrown when API validation fails.
/// </summary>
public class ValidationException : ApiClientException
{
    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    public ValidationException(IDictionary<string, string[]> errors)
        : base(BuildErrorMessage(errors), HttpStatusCode.BadRequest)
    {
        Errors = new Dictionary<string, string[]>(errors);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The validation errors.</param>
    public ValidationException(string message, IDictionary<string, string[]> errors)
        : base(message, HttpStatusCode.BadRequest)
    {
        Errors = new Dictionary<string, string[]>(errors);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The validation errors.</param>
    /// <param name="responseContent">The response content.</param>
    /// <param name="requestUri">The request URI.</param>
    public ValidationException(
        string message,
        IDictionary<string, string[]> errors,
        string? responseContent = null,
        string? requestUri = null)
        : base(message, HttpStatusCode.BadRequest, responseContent, requestUri)
    {
        Errors = new Dictionary<string, string[]>(errors);
    }

    private static string BuildErrorMessage(IDictionary<string, string[]> errors)
    {
        if (errors == null || !errors.Any())
        {
            return "Validation failed.";
        }

        var errorMessages = errors
            .SelectMany(kvp => kvp.Value.Select(error => $"{kvp.Key}: {error}"))
            .ToList();

        return $"Validation failed with {errorMessages.Count} error(s):\n- " + 
               string.Join("\n- ", errorMessages);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var baseMessage = base.ToString();

        if (Errors.Any())
        {
            baseMessage += "\nValidation Errors:";
            foreach (var error in Errors)
            {
                baseMessage += $"\n  {error.Key}:";
                foreach (var message in error.Value)
                {
                    baseMessage += $"\n    - {message}";
                }
            }
        }

        return baseMessage;
    }
}
