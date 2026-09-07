using Baseera.Core.Documents.Enums;

namespace Baseera.Core.KnowledgeExtraction.Models
{
    public sealed class ExtractedRelationship
    {
        public string SourceId { get; init; } = null!;

        public RelationshipType Relationship { get; init; }

        public string TargetId { get; init; } = null!;

        public string? Evidence { get; init; }

        public double Confidence { get; init; }
    }
}
