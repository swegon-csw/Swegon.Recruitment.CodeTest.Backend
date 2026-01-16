using System.ComponentModel.DataAnnotations;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

public class SearchRequest
{
    [Required]
    [StringLength(500)]
    public string Query { get; set; } = string.Empty;

    public List<string>? SearchFields { get; set; }

    [Range(1, 1000)]
    public int MaxResults { get; set; } = 50;

    [Range(0, 100)]
    public int MinScore { get; set; } = 0;

    public bool FuzzyMatch { get; set; }
}
