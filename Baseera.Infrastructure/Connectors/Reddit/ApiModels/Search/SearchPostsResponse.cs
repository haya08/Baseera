namespace Baseera.Service.Connectors.Reddit.Models.Search
{
    public sealed class SearchPostsResponse
    {
        public required IReadOnlyList<SearchPostItem> Posts { get; init; }

        public string? NextCursor { get; init; }
    }

    public sealed class SearchPostItem
    {
        public required string PostId { get; init; }

        public required string Title { get; init; }
    }
}
