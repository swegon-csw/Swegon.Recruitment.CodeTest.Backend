using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Common;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

/// <summary>
/// Generic paged response wrapper
/// </summary>
/// <typeparam name="T">Type of items in the response</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// Items in the current page
    /// </summary>
    public List<T> Items { get; set; } = new();
    
    /// <summary>
    /// Pagination metadata
    /// </summary>
    public PaginationMetadata Pagination { get; set; } = new();
    
    /// <summary>
    /// Total count of items across all pages
    /// </summary>
    public int TotalCount => Pagination.TotalCount;
    
    /// <summary>
    /// Number of items in current page
    /// </summary>
    public int Count => Items.Count;
}
