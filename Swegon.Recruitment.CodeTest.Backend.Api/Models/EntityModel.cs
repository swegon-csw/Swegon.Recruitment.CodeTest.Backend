using System.ComponentModel.DataAnnotations;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

/// <summary>
/// Base entity model with common properties
/// </summary>
public abstract class EntityModel
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// When the entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the entity was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Who created the entity
    /// </summary>
    public string? CreatedBy { get; set; }
    
    /// <summary>
    /// Who last updated the entity
    /// </summary>
    public string? UpdatedBy { get; set; }
    
    /// <summary>
    /// Whether the entity is soft deleted
    /// </summary>
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// When the entity was deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
