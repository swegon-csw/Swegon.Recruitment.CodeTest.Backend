namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Common;

/// <summary>
/// Generic API result wrapper
/// </summary>
/// <typeparam name="T">Type of data being returned</typeparam>
public class ApiResult<T>
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// The result data
    /// </summary>
    public T? Data { get; set; }
    
    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Error code if operation failed
    /// </summary>
    public string? ErrorCode { get; set; }
    
    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static ApiResult<T> Ok(T data) => new()
    {
        Success = true,
        Data = data
    };
    
    /// <summary>
    /// Creates a failed result
    /// </summary>
    public static ApiResult<T> Fail(string errorMessage, string? errorCode = null) => new()
    {
        Success = false,
        ErrorMessage = errorMessage,
        ErrorCode = errorCode
    };
}
