using System.ComponentModel.DataAnnotations;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

public class ConfigurationRequest
{
    [Required]
    [StringLength(100)]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Value { get; set; } = string.Empty;

    public ConfigurationType Type { get; set; } = ConfigurationType.Global;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsSensitive { get; set; }

    [StringLength(50)]
    public string? Category { get; set; }
}
