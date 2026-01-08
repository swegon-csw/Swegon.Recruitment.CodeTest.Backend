using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Common;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Extensions;

/// <summary>
/// Extension methods for model transformations
/// </summary>
public static class ModelExtensions
{
    /// <summary>
    /// Converts a list to a paged response
    /// </summary>
    public static PagedResponse<T> ToPagedResponse<T>(this IEnumerable<T> items, int currentPage, int pageSize, int totalCount)
    {
        return new PagedResponse<T>
        {
            Items = items.ToList(),
            Pagination = new PaginationMetadata
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = totalCount
            }
        };
    }
    
    /// <summary>
    /// Wraps data in an API result
    /// </summary>
    public static ApiResult<T> ToApiResult<T>(this T data)
    {
        return ApiResult<T>.Ok(data);
    }
    
    /// <summary>
    /// Creates an error API result
    /// </summary>
    public static ApiResult<T> ToErrorResult<T>(string errorMessage, string? errorCode = null)
    {
        return ApiResult<T>.Fail(errorMessage, errorCode);
    }
}
