using Baseera.Core.Documents.Enums;

namespace Baseera.Core.KnowledgeExtraction.Models
{
    public sealed class ExtractedEntity
    {
        public string Id { get; init; } = null!;

        public string Name { get; init; } = null!;

        public EntityType Type { get; init; }

        public string? Description { get; init; }

        public string? Evidence { get; init; }

        public double Confidence { get; init; }
    }
}
