using System.Globalization;
using System.Text;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Helpers;

/// <summary>
/// Helper class for formatting utilities
/// </summary>
public static class FormatHelper
{
    /// <summary>
    /// Formats a decimal as currency
    /// </summary>
    public static string FormatCurrency(decimal amount, string currencyCode = "USD")
    {
        var culture = GetCultureForCurrency(currencyCode);
        return amount.ToString("C", culture);
    }
    
    /// <summary>
    /// Formats a number with thousand separators
    /// </summary>
    public static string FormatNumber(decimal number, int decimalPlaces = 2)
    {
        return number.ToString($"N{decimalPlaces}", CultureInfo.InvariantCulture);
    }
    
    /// <summary>
    /// Formats a percentage
    /// </summary>
    public static string FormatPercentage(decimal percentage, int decimalPlaces = 2)
    {
        return $"{percentage.ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture)}%";
    }
    
    /// <summary>
    /// Formats a DateTime to ISO 8601 format
    /// </summary>
    public static string FormatDateTimeIso(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
    }
    
    /// <summary>
    /// Formats a DateTime to a readable format
    /// </summary>
    public static string FormatDateTimeReadable(DateTime dateTime)
    {
        return dateTime.ToString("MMMM dd, yyyy h:mm tt", CultureInfo.InvariantCulture);
    }
    
    /// <summary>
    /// Formats a TimeSpan to a readable format
    /// </summary>
    public static string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalDays >= 1)
            return $"{duration.Days}d {duration.Hours}h {duration.Minutes}m";
        if (duration.TotalHours >= 1)
            return $"{duration.Hours}h {duration.Minutes}m {duration.Seconds}s";
        if (duration.TotalMinutes >= 1)
            return $"{duration.Minutes}m {duration.Seconds}s";
        if (duration.TotalSeconds >= 1)
            return $"{duration.Seconds}s";
        return $"{duration.Milliseconds}ms";
    }
    
    /// <summary>
    /// Formats file size in human-readable format
    /// </summary>
    public static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
    
    /// <summary>
    /// Formats a phone number
    /// </summary>
    public static string FormatPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return string.Empty;
        
        var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());
        
        if (digits.Length == 10)
            return $"({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6)}";
        if (digits.Length == 11)
            return $"+{digits[0]} ({digits.Substring(1, 3)}) {digits.Substring(4, 3)}-{digits.Substring(7)}";
        
        return phoneNumber;
    }
    
    /// <summary>
    /// Truncates a string with ellipsis
    /// </summary>
    public static string Truncate(string text, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            return text;
        
        return text.Substring(0, maxLength - suffix.Length) + suffix;
    }
    
    /// <summary>
    /// Converts a string to title case
    /// </summary>
    public static string ToTitleCase(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;
        
        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text.ToLower());
    }
    
    /// <summary>
    /// Converts a string to slug format (lowercase with hyphens)
    /// </summary>
    public static string ToSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;
        
        var slug = text.ToLowerInvariant();
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", " ").Trim();
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s", "-");
        
        return slug;
    }
    
    /// <summary>
    /// Formats a credit card number with masking
    /// </summary>
    public static string FormatCreditCard(string cardNumber, bool mask = true)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return string.Empty;
        
        var digits = new string(cardNumber.Where(char.IsDigit).ToArray());
        
        if (digits.Length < 13 || digits.Length > 16)
            return cardNumber;
        
        if (mask && digits.Length >= 4)
        {
            var lastFour = digits.Substring(digits.Length - 4);
            return $"**** **** **** {lastFour}";
        }
        
        var formatted = new StringBuilder();
        for (int i = 0; i < digits.Length; i++)
        {
            if (i > 0 && i % 4 == 0)
                formatted.Append(' ');
            formatted.Append(digits[i]);
        }
        
        return formatted.ToString();
    }
    
    /// <summary>
    /// Formats a Guid for display (first 8 characters)
    /// </summary>
    public static string FormatGuidShort(Guid guid)
    {
        return guid.ToString().Substring(0, 8);
    }
    
    /// <summary>
    /// Pluralizes a word based on count
    /// </summary>
    public static string Pluralize(string word, int count)
    {
        if (count == 1)
            return word;
        
        // Simple pluralization rules
        if (word.EndsWith("y") && !word.EndsWith("ay") && !word.EndsWith("ey") && !word.EndsWith("oy") && !word.EndsWith("uy"))
            return word.Substring(0, word.Length - 1) + "ies";
        if (word.EndsWith("s") || word.EndsWith("x") || word.EndsWith("z") || word.EndsWith("ch") || word.EndsWith("sh"))
            return word + "es";
        
        return word + "s";
    }
    
    /// <summary>
    /// Gets culture info for currency code
    /// </summary>
    private static CultureInfo GetCultureForCurrency(string currencyCode)
    {
        return currencyCode.ToUpperInvariant() switch
        {
            "USD" => new CultureInfo("en-US"),
            "EUR" => new CultureInfo("de-DE"),
            "GBP" => new CultureInfo("en-GB"),
            "JPY" => new CultureInfo("ja-JP"),
            "SEK" => new CultureInfo("sv-SE"),
            _ => CultureInfo.InvariantCulture
        };
    }
}
