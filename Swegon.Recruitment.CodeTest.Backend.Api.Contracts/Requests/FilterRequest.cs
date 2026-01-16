using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Common;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Requests;

public class FilterRequest : FilterCriteria
{
    public bool IncludeInactive { get; set; }

    public List<Guid>? Ids { get; set; }

    public List<string>? Tags { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }
}
