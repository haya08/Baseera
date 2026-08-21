namespace Baseera.Infrastructure.Connectors.Documents
{
    public sealed class UnifiedComment
    {
        public required string ExternalId { get; init; }

        public required string Author { get; init; }

        public required string Body { get; init; }

        public DateTimeOffset PublishedAt { get; init; }

        public int Score { get; init; }

        public int Depth { get; init; }

        public string? ParentId { get; init; }
    }
}
