namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

/// <summary>
/// Response for search operations
/// </summary>
/// <typeparam name="T">Type of search results</typeparam>
public class SearchResponse<T>
{
    /// <summary>
    /// Search results
    /// </summary>
    public List<SearchResult<T>> Results { get; set; } = new();
    
    /// <summary>
    /// Total number of results found
    /// </summary>
    public int TotalResults { get; set; }
    
    /// <summary>
    /// Search query that was executed
    /// </summary>
    public string Query { get; set; } = string.Empty;
    
    /// <summary>
    /// Search execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }
    
    /// <summary>
    /// Suggested queries for refinement
    /// </summary>
    public List<string>? Suggestions { get; set; }
}

/// <summary>
/// Individual search result with score
/// </summary>
/// <typeparam name="T">Type of the result item</typeparam>
public class SearchResult<T>
{
    /// <summary>
    /// The result item
    /// </summary>
    public T Item { get; set; } = default!;
    
    /// <summary>
    /// Match score (0-100)
    /// </summary>
    public int Score { get; set; }
    
    /// <summary>
    /// Highlighted fields
    /// </summary>
    public Dictionary<string, string>? Highlights { get; set; }
}
