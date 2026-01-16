namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts;

public class ProductDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public string? ImageUrl { get; set; }
    public bool InStock { get; set; }
    public double? Rating { get; set; }
    public int? ReviewCount { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
    public string UpdatedAt { get; set; } = string.Empty;
}
