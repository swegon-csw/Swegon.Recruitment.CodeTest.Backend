using System.ComponentModel.DataAnnotations;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

public class Configuration : EntityModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public ConfigurationType Type { get; set; }

    [Required]
    public string Value { get; set; } = string.Empty;

    [StringLength(50)]
    public string Category { get; set; } = "General";

    public bool IsActive { get; set; } = true;

    public bool IsReadOnly { get; set; }

    [StringLength(50)]
    public string Environment { get; set; } = "Production";

    [StringLength(50)]
    public string DataType { get; set; } = "String";

    public Dictionary<string, object>? ValidationRules { get; set; }

    public List<string> Tags { get; set; } = new();

    public int Version { get; set; } = 1;

    public string? PreviousValue { get; set; }

    public DateTime? EffectiveDate { get; set; }

    public DateTime? ExpirationDate { get; set; }
}
