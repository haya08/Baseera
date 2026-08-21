using Baseera.Core.Documents.Enums;

namespace Baseera.Core.Documents.Models
{
    public sealed class KnowledgeExtractionResult
    {
        public IReadOnlyList<EntityCandidate> Entities { get; init; }
            = [];

        public IReadOnlyList<RelationshipCandidate> Relationships { get; init; }
            = [];
    }

    public sealed class EntityCandidate
    {
        public string Name { get; init; } = null!;

        public EntityType Type { get; init; }

        public string? Description { get; init; }

        public string? Evidence { get; init; }

        public double Confidence { get; init; }
    }

    public sealed class RelationshipCandidate
    {
        public string Source { get; init; } = null!;

        public RelationshipType Relationship { get; init; }

        public string Target { get; init; } = null!;

        public string? Evidence { get; init; }

        public double Confidence { get; init; }
    }
}
