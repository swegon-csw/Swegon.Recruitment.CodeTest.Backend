namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

/// <summary>
/// Generic data model for flexible data structures
/// </summary>
public class DataModel
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Data type identifier
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Data name/label
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Data properties as key-value pairs
    /// </summary>
    public Dictionary<string, object> Properties { get; set; } = new();
    
    /// <summary>
    /// Metadata about the data
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
    
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Last modified timestamp
    /// </summary>
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Version number for optimistic concurrency
    /// </summary>
    public int Version { get; set; } = 1;
    
    /// <summary>
    /// Gets a property value by key
    /// </summary>
    public T? GetProperty<T>(string key)
    {
        if (Properties.TryGetValue(key, out var value))
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }
        return default;
    }
    
    /// <summary>
    /// Sets a property value
    /// </summary>
    public void SetProperty(string key, object value)
    {
        Properties[key] = value;
        ModifiedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Checks if a property exists
    /// </summary>
    public bool HasProperty(string key) => Properties.ContainsKey(key);
    
    /// <summary>
    /// Removes a property
    /// </summary>
    public bool RemoveProperty(string key)
    {
        if (Properties.Remove(key))
        {
            ModifiedAt = DateTime.UtcNow;
            return true;
        }
        return false;
    }
}
