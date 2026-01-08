using System.ComponentModel.DataAnnotations;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

/// <summary>
/// Request for data export
/// </summary>
public class ExportRequest
{
    /// <summary>
    /// Export format (csv, json, xml)
    /// </summary>
    [Required]
    public string Format { get; set; } = "json";
    
    /// <summary>
    /// Fields to include in export
    /// </summary>
    public List<string>? Fields { get; set; }
    
    /// <summary>
    /// Filter criteria for export
    /// </summary>
    public FilterRequest? Filter { get; set; }
    
    /// <summary>
    /// Include metadata in export
    /// </summary>
    public bool IncludeMetadata { get; set; }
    
    /// <summary>
    /// Compress export file
    /// </summary>
    public bool Compress { get; set; }
}
