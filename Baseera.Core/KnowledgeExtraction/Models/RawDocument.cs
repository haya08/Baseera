using Baseera.Core.Documents.Enums;

namespace Baseera.Core.Documents.Models
{
    public sealed class RawDocument
    {
        public Guid Id { get; set; }

        // Identifier from the original source
        public string ExternalId { get; set; } = null!;

        // Where the document came from
        public DocumentSource Source { get; set; }

        // What kind of content it represents
        public DocumentType Type { get; set; }

        // Main textual content
        public string Content { get; set; } = null!;

        // Optional title
        public string? Title { get; set; }

        // Original source URL
        public string? Url { get; set; }

        // Original author/creator if available
        public string? Author { get; set; }

        // Original publication/creation date
        public DateTimeOffset? PublishedAt { get; set; }

        // When Baseera collected the document
        public DateTimeOffset CollectedAt { get; set; }

        // Detected/provided language
        public string? Language { get; set; }

        // Source-specific information
        public Dictionary<string, object>? Metadata { get; set; }
    }
}
