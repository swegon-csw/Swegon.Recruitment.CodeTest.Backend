using System.ComponentModel.DataAnnotations;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

public class BatchRequest<T>
    where T : class
{
    [Required]
    [StringLength(20)]
    public string Operation { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public List<T> Items { get; set; } = new();

    public bool ContinueOnError { get; set; }

    public bool ValidateBeforeProcessing { get; set; } = true;

    public Dictionary<string, string>? Metadata { get; set; }
}
