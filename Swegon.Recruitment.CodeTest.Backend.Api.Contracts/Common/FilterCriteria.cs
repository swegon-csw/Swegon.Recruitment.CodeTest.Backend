namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Common;

public class FilterCriteria
{
    public string? SearchTerm { get; set; }

    public string? SortBy { get; set; }

    public string? SortDirection { get; set; } = "asc";

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
