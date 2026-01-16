using System.ComponentModel.DataAnnotations;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

public class ExportRequest
{
    [Required]
    public string Format { get; set; } = "json";

    public List<string>? Fields { get; set; }

    public FilterRequest? Filter { get; set; }

    public bool IncludeMetadata { get; set; }

    public bool Compress { get; set; }
}
