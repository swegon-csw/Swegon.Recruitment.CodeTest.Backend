using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

public class Result<T>
{
    public bool IsSuccess { get; set; }

    public T? Data { get; set; }

    public string? ErrorMessage { get; set; }

    public ErrorCode? ErrorCode { get; set; }

    public Dictionary<string, object>? ErrorDetails { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public long ExecutionTimeMs { get; set; }

    public static Result<T> Success(T data)
    {
        return new Result<T> { IsSuccess = true, Data = data };
    }

    public static Result<T> Failure(
        string errorMessage,
        ErrorCode errorCode = Contracts.Enums.ErrorCode.InternalError
    )
    {
        return new Result<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode,
        };
    }

    public static Result<T> Failure(
        string errorMessage,
        ErrorCode errorCode,
        Dictionary<string, object> errorDetails
    )
    {
        return new Result<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode,
            ErrorDetails = errorDetails,
        };
    }
}

public class Result
{
    public bool IsSuccess { get; set; }

    public string? ErrorMessage { get; set; }

    public ErrorCode? ErrorCode { get; set; }

    public Dictionary<string, object>? ErrorDetails { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static Result Success()
    {
        return new Result { IsSuccess = true };
    }

    public static Result Failure(
        string errorMessage,
        ErrorCode errorCode = Contracts.Enums.ErrorCode.InternalError
    )
    {
        return new Result
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode,
        };
    }
}
