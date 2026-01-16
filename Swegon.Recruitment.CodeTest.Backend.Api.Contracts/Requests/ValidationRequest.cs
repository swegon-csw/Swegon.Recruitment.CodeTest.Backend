using System.ComponentModel.DataAnnotations;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

public class ValidationRequest
{
    [Required]
    public Dictionary<string, object> Data { get; set; } = new();

    public List<string>? Rules { get; set; }

    public bool StopOnFirstError { get; set; }

    public string? Context { get; set; }
}
