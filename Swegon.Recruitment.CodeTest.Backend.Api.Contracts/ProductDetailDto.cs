namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts;

public class ProductDetailDto : ProductDto
{
    public List<ProductSpecification> Specifications { get; set; } = new();
    public List<string> Features { get; set; } = new();
    public string Manufacturer { get; set; } = string.Empty;
    public string Warranty { get; set; } = string.Empty;
    public List<ProductDto>? RelatedProducts { get; set; }
}
