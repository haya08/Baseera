using Baseera.Core.Documents.Enums;

namespace Baseera.Core.KnowledgeResolution.Models
{
    public sealed class ResolvedRelationship
    {
        public string ExtractedRelationshipId { get; init; } = null!;

        public Guid SourceEntityId { get; init; }

        public RelationshipType Relationship { get; init; }

        public Guid TargetEntityId { get; init; }
    }
}
