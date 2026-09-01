using Baseera.Domain.Enums;

namespace Baseera.Domain.Entities
{
    public class KnowledgeRelationship
    {
        public Guid Id { get; set; }

        public Guid SourceEntityId { get; set; }

        public Guid TargetEntityId { get; set; }

        public RelationshipType Type { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
