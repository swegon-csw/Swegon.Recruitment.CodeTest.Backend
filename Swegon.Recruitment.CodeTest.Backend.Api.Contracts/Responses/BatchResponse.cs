namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

/// <summary>
/// Response for batch operations
/// </summary>
/// <typeparam name="T">Type of items processed</typeparam>
public class BatchResponse<T>
{
    /// <summary>
    /// Successfully processed items
    /// </summary>
    public List<T> Successful { get; set; } = new();
    
    /// <summary>
    /// Failed items with errors
    /// </summary>
    public List<BatchError<T>> Failed { get; set; } = new();
    
    /// <summary>
    /// Total items processed
    /// </summary>
    public int TotalProcessed { get; set; }
    
    /// <summary>
    /// Number of successful operations
    /// </summary>
    public int SuccessCount => Successful.Count;
    
    /// <summary>
    /// Number of failed operations
    /// </summary>
    public int FailureCount => Failed.Count;
    
    /// <summary>
    /// Processing duration in milliseconds
    /// </summary>
    public long DurationMs { get; set; }
}

/// <summary>
/// Information about a failed batch item
/// </summary>
/// <typeparam name="T">Type of the failed item</typeparam>
public class BatchError<T>
{
    /// <summary>
    /// The item that failed
    /// </summary>
    public T Item { get; set; } = default!;
    
    /// <summary>
    /// Error message
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// Error code
    /// </summary>
    public string? ErrorCode { get; set; }
    
    /// <summary>
    /// Index of the item in the batch
    /// </summary>
    public int Index { get; set; }
}
