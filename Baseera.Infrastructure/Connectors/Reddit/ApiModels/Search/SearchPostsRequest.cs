namespace Baseera.Service.Connectors.Reddit.Models.Search
{
    public sealed class SearchPostsRequest
    {
        public required string Query { get; init; }

        public int Limit { get; init; } = 25;

        public string Sort { get; init; } = "relevance";

        public string TimeFilter { get; init; } = "all";

        public string? Cursor { get; init; }
    }
}
