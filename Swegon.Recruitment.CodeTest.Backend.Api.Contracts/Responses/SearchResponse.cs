namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Responses;

public class SearchResponse<T>
{
    public List<SearchResult<T>> Results { get; set; } = new();
    public int TotalResults { get; set; }
    public string Query { get; set; } = string.Empty;
    public long ExecutionTimeMs { get; set; }
    public List<string>? Suggestions { get; set; }
}

public class SearchResult<T>
{
    public T Item { get; set; } = default!;
    public int Score { get; set; }
    public Dictionary<string, string>? Highlights { get; set; }
}
