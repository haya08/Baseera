namespace Baseera.Core.KnowledgeResolution.Models
{
    public sealed class ResolvedKnowledge
    {
        public IReadOnlyList<ResolvedEntity> Entities { get; init; }
            = [];

        public IReadOnlyList<ResolvedRelationship> Relationships { get; init; }
            = [];
    }
}
