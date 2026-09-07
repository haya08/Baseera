namespace Baseera.Core.KnowledgeExtraction.Models
{
    public sealed class KnowledgeExtractionResult
    {
        public IReadOnlyList<ExtractedEntity> Entities { get; init; }
            = [];

        public IReadOnlyList<ExtractedRelationship> Relationships { get; init; }
            = [];
    }
}
