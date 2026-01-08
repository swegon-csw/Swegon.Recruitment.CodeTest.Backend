namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Common;

/// <summary>
/// Filter criteria for querying data
/// </summary>
public class FilterCriteria
{
    /// <summary>
    /// Search term for text-based filtering
    /// </summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Field to sort by
    /// </summary>
    public string? SortBy { get; set; }
    
    /// <summary>
    /// Sort direction (asc/desc)
    /// </summary>
    public string? SortDirection { get; set; } = "asc";
    
    /// <summary>
    /// Page number for pagination (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;
    
    /// <summary>
    /// Page size for pagination
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// Start date for date range filtering
    /// </summary>
    public DateTime? StartDate { get; set; }
    
    /// <summary>
    /// End date for date range filtering
    /// </summary>
    public DateTime? EndDate { get; set; }
}
