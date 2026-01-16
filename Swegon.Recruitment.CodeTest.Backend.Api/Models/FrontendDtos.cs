namespace Swegon.Recruitment.CodeTest.Backend.Api.Models;

public class ProductSpecification
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Unit { get; set; }
}

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

public class ProductDetailDto : ProductDto
{
    public List<ProductSpecification> Specifications { get; set; } = new();
    public List<string> Features { get; set; } = new();
    public string Manufacturer { get; set; } = string.Empty;
    public string Warranty { get; set; } = string.Empty;
    public List<ProductDto>? RelatedProducts { get; set; }
}

public class FrontendPaginationMetadata
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
}

public class PaginatedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public FrontendPaginationMetadata Pagination { get; set; } = new();
}

public class CalculationInputDto
{
    public double Area { get; set; }
    public double Height { get; set; }
    public int Occupancy { get; set; }
    public string ActivityLevel { get; set; } = "medium";
    public double Temperature { get; set; }
    public double Humidity { get; set; }
}

public class CalculationCost
{
    public int Installation { get; set; }
    public int Annual { get; set; }
    public int Lifetime { get; set; }
}

public class CalculationResultDto
{
    public double Airflow { get; set; }
    public int CoolingCapacity { get; set; }
    public int HeatingCapacity { get; set; }
    public double EnergyConsumption { get; set; }
    public CalculationCost Cost { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public class CalculationHistoryDto
{
    public string Id { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
    public string? Name { get; set; }
    public CalculationInputDto Input { get; set; } = new();
    public CalculationResultDto Result { get; set; } = new();
}
