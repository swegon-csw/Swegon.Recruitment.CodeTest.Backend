namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

public class DataModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Type { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public Dictionary<string, object> Properties { get; set; } = new();

    public Dictionary<string, string> Metadata { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public int Version { get; set; } = 1;

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

    public void SetProperty(string key, object value)
    {
        Properties[key] = value;
        ModifiedAt = DateTime.UtcNow;
    }

    public bool HasProperty(string key) => Properties.ContainsKey(key);

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
