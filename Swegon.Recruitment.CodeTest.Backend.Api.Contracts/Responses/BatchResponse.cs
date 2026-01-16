namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

public class BatchResponse<T>
{
    public List<T> Successful { get; set; } = new();
    public List<BatchError<T>> Failed { get; set; } = new();
    public int TotalProcessed { get; set; }
    public int SuccessCount => Successful.Count;
    public int FailureCount => Failed.Count;
    public long DurationMs { get; set; }
}

public class BatchError<T>
{
    public T Item { get; set; } = default!;
    public string ErrorMessage { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public int Index { get; set; }
}
