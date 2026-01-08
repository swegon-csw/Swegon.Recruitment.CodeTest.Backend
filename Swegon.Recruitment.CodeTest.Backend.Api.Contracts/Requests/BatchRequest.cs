using System.ComponentModel.DataAnnotations;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

/// <summary>
/// Request for batch operations
/// </summary>
public class BatchRequest<T> where T : class
{
    /// <summary>
    /// Operation type (create, update, delete)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Operation { get; set; } = string.Empty;
    
    /// <summary>
    /// Items to process in batch
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<T> Items { get; set; } = new();
    
    /// <summary>
    /// Whether to continue on error
    /// </summary>
    public bool ContinueOnError { get; set; }
    
    /// <summary>
    /// Whether to validate all items before processing
    /// </summary>
    public bool ValidateBeforeProcessing { get; set; } = true;
    
    /// <summary>
    /// Batch metadata
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
}
