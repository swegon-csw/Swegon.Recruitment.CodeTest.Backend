using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Common;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

/// <summary>
/// Request for filtering data
/// </summary>
public class FilterRequest : FilterCriteria
{
    /// <summary>
    /// Include inactive items
    /// </summary>
    public bool IncludeInactive { get; set; }
    
    /// <summary>
    /// Filter by IDs
    /// </summary>
    public List<Guid>? Ids { get; set; }
    
    /// <summary>
    /// Filter by tags
    /// </summary>
    public List<string>? Tags { get; set; }
    
    /// <summary>
    /// Minimum price filter
    /// </summary>
    public decimal? MinPrice { get; set; }
    
    /// <summary>
    /// Maximum price filter
    /// </summary>
    public decimal? MaxPrice { get; set; }
}
