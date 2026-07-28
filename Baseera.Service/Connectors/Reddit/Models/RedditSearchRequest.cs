namespace Baseera.Service.Connectors.Reddit.Models
{
    public sealed class RedditSearchRequest
    {
        public required string Query { get; init; }

        public string? After { get; init; }

        public int Limit { get; init; } = 25;

        public string Sort { get; init; } = "relevance";

        public string Time { get; init; } = "all";
    }
}
