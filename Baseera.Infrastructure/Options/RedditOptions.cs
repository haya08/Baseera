namespace Baseera.Infrastructure.Options
{
    public sealed class RedditOptions
    {
        public const string SectionName = "Reddit";

        public required string BaseUrl { get; init; }

        public required string ApiKey { get; init; }

        public required string ApiHost { get; init; }

        public required string SearchEndpoint { get; init; }
    }
}
