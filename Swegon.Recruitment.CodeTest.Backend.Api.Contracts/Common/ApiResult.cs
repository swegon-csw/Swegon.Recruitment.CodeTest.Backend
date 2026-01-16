namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Common;

public class ApiResult<T>
{
    public bool Success { get; set; }

    public T? Data { get; set; }

    public string? ErrorMessage { get; set; }

    public string? ErrorCode { get; set; }

    public static ApiResult<T> Ok(T data) => new() { Success = true, Data = data };

    public static ApiResult<T> Fail(string errorMessage, string? errorCode = null) =>
        new()
        {
            Success = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode,
        };
}
