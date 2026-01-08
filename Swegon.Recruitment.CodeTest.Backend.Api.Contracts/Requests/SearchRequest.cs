using System.ComponentModel.DataAnnotations;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

/// <summary>
/// Request for search operations
/// </summary>
public class SearchRequest
{
    /// <summary>
    /// Search query
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Query { get; set; } = string.Empty;
    
    /// <summary>
    /// Fields to search in
    /// </summary>
    public List<string>? SearchFields { get; set; }
    
    /// <summary>
    /// Maximum results to return
    /// </summary>
    [Range(1, 1000)]
    public int MaxResults { get; set; } = 50;
    
    /// <summary>
    /// Minimum match score (0-100)
    /// </summary>
    [Range(0, 100)]
    public int MinScore { get; set; } = 0;
    
    /// <summary>
    /// Whether to include fuzzy matching
    /// </summary>
    public bool FuzzyMatch { get; set; }
}
