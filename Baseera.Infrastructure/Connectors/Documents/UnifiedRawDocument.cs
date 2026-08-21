using Baseera.Domain.Enums;

namespace Baseera.Infrastructure.Connectors.Documents
{
    public sealed class UnifiedRawDocument
    {
        // Identity
        public required string ExternalId { get; init; }
        public required Platform Platform { get; init; }

        // Main Content
        public string? Title { get; init; }
        public string? Body { get; init; }
        public IReadOnlyList<UnifiedComment> Comments { get; init; } = [];

        // Source Information
        public string? Author { get; init; }
        public string? Url { get; init; }
        public DateTimeOffset PublishedAt { get; init; }
        public DateTimeOffset CollectedAt { get; init; }

        // Platform-specific metadata
        public IReadOnlyDictionary<string, string> Metadata { get; init; }
            = new Dictionary<string, string>();

        // Original payload
        public required string RawJson { get; init; }
    }
}
