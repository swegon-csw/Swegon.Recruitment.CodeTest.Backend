namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

/// <summary>
/// Response for export operations
/// </summary>
public class ExportResponse
{
    /// <summary>
    /// Export file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    
    /// <summary>
    /// Export format
    /// </summary>
    public string Format { get; set; } = string.Empty;
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }
    
    /// <summary>
    /// Number of records exported
    /// </summary>
    public int RecordCount { get; set; }
    
    /// <summary>
    /// Download URL for the exported file
    /// </summary>
    public string? DownloadUrl { get; set; }
    
    /// <summary>
    /// Export generation timestamp
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// URL expiration time if applicable
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}
