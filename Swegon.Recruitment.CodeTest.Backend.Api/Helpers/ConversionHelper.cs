using System.Globalization;
using System.Text.Json;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Helpers;

/// <summary>
/// Helper class for type conversions and data transformations
/// </summary>
public static class ConversionHelper
{
    /// <summary>
    /// Converts an object to a specific type
    /// </summary>
    public static T? ConvertTo<T>(object? value)
    {
        if (value == null)
            return default;
        
        if (value is T typedValue)
            return typedValue;
        
        try
        {
            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
        catch
        {
            return default;
        }
    }
    
    /// <summary>
    /// Safely converts a string to an integer
    /// </summary>
    public static int? ToInt(string? value, int? defaultValue = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;
        
        return int.TryParse(value, out var result) ? result : defaultValue;
    }
    
    /// <summary>
    /// Safely converts a string to a decimal
    /// </summary>
    public static decimal? ToDecimal(string? value, decimal? defaultValue = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;
        
        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;
    }
    
    /// <summary>
    /// Safely converts a string to a boolean
    /// </summary>
    public static bool? ToBool(string? value, bool? defaultValue = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;
        
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }
    
    /// <summary>
    /// Safely converts a string to a DateTime
    /// </summary>
    public static DateTime? ToDateTime(string? value, DateTime? defaultValue = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;
        
        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) ? result : defaultValue;
    }
    
    /// <summary>
    /// Safely converts a string to a Guid
    /// </summary>
    public static Guid? ToGuid(string? value, Guid? defaultValue = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;
        
        return Guid.TryParse(value, out var result) ? result : defaultValue;
    }
    
    /// <summary>
    /// Converts an object to a JSON string
    /// </summary>
    public static string ToJson(object? value)
    {
        if (value == null)
            return "null";
        
        return JsonSerializer.Serialize(value);
    }
    
    /// <summary>
    /// Converts a JSON string to an object
    /// </summary>
    public static T? FromJson<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return default;
        
        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return default;
        }
    }
    
    /// <summary>
    /// Converts bytes to megabytes
    /// </summary>
    public static decimal BytesToMegabytes(long bytes)
    {
        return bytes / (1024m * 1024m);
    }
    
    /// <summary>
    /// Converts megabytes to bytes
    /// </summary>
    public static long MegabytesToBytes(decimal megabytes)
    {
        return (long)(megabytes * 1024m * 1024m);
    }
    
    /// <summary>
    /// Converts celsius to fahrenheit
    /// </summary>
    public static decimal CelsiusToFahrenheit(decimal celsius)
    {
        return (celsius * 9m / 5m) + 32m;
    }
    
    /// <summary>
    /// Converts fahrenheit to celsius
    /// </summary>
    public static decimal FahrenheitToCelsius(decimal fahrenheit)
    {
        return (fahrenheit - 32m) * 5m / 9m;
    }
    
    /// <summary>
    /// Converts kilograms to pounds
    /// </summary>
    public static decimal KilogramsToPounds(decimal kilograms)
    {
        return kilograms * 2.20462m;
    }
    
    /// <summary>
    /// Converts pounds to kilograms
    /// </summary>
    public static decimal PoundsToKilograms(decimal pounds)
    {
        return pounds / 2.20462m;
    }
    
    /// <summary>
    /// Converts meters to feet
    /// </summary>
    public static decimal MetersToFeet(decimal meters)
    {
        return meters * 3.28084m;
    }
    
    /// <summary>
    /// Converts feet to meters
    /// </summary>
    public static decimal FeetToMeters(decimal feet)
    {
        return feet / 3.28084m;
    }
    
    /// <summary>
    /// Converts a dictionary to a query string
    /// </summary>
    public static string DictionaryToQueryString(Dictionary<string, string> parameters)
    {
        if (parameters == null || parameters.Count == 0)
            return string.Empty;
        
        var queryString = string.Join("&", parameters.Select(kvp => 
            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
        
        return $"?{queryString}";
    }
    
    /// <summary>
    /// Parses a query string to a dictionary
    /// </summary>
    public static Dictionary<string, string> QueryStringToDictionary(string queryString)
    {
        var result = new Dictionary<string, string>();
        
        if (string.IsNullOrWhiteSpace(queryString))
            return result;
        
        queryString = queryString.TrimStart('?');
        var pairs = queryString.Split('&');
        
        foreach (var pair in pairs)
        {
            var parts = pair.Split('=');
            if (parts.Length == 2)
            {
                result[Uri.UnescapeDataString(parts[0])] = Uri.UnescapeDataString(parts[1]);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Converts enum to string
    /// </summary>
    public static string EnumToString<T>(T enumValue) where T : Enum
    {
        return enumValue.ToString();
    }
    
    /// <summary>
    /// Converts string to enum
    /// </summary>
    public static T? StringToEnum<T>(string? value, T? defaultValue = default) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;
        
        return Enum.TryParse<T>(value, true, out var result) ? result : defaultValue;
    }
}
