using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

/// <summary>
/// Generic result wrapper for operation results
/// </summary>
/// <typeparam name="T">Type of the result data</typeparam>
public class Result<T>
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; set; }
    
    /// <summary>
    /// Result data if successful
    /// </summary>
    public T? Data { get; set; }
    
    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Error code if operation failed
    /// </summary>
    public ErrorCode? ErrorCode { get; set; }
    
    /// <summary>
    /// Additional error details
    /// </summary>
    public Dictionary<string, object>? ErrorDetails { get; set; }
    
    /// <summary>
    /// Timestamp of the operation
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }
    
    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static Result<T> Success(T data)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Data = data
        };
    }
    
    /// <summary>
    /// Creates a failed result
    /// </summary>
    public static Result<T> Failure(string errorMessage, ErrorCode errorCode = Contracts.Enums.ErrorCode.InternalError)
    {
        return new Result<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode
        };
    }
    
    /// <summary>
    /// Creates a failed result with details
    /// </summary>
    public static Result<T> Failure(string errorMessage, ErrorCode errorCode, Dictionary<string, object> errorDetails)
    {
        return new Result<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode,
            ErrorDetails = errorDetails
        };
    }
}

/// <summary>
/// Non-generic result for operations that don't return data
/// </summary>
public class Result
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; set; }
    
    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Error code if operation failed
    /// </summary>
    public ErrorCode? ErrorCode { get; set; }
    
    /// <summary>
    /// Additional error details
    /// </summary>
    public Dictionary<string, object>? ErrorDetails { get; set; }
    
    /// <summary>
    /// Timestamp of the operation
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static Result Success()
    {
        return new Result { IsSuccess = true };
    }
    
    /// <summary>
    /// Creates a failed result
    /// </summary>
    public static Result Failure(string errorMessage, ErrorCode errorCode = Contracts.Enums.ErrorCode.InternalError)
    {
        return new Result
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode
        };
    }
}
