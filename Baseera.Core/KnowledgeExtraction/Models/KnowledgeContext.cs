using Baseera.Domain.Entities;

namespace Baseera.Core.KnowledgeExtraction.Models
{
    public class KnowledgeContext
    {
        public IReadOnlyList<BaseEntity> Entities { get; set; }
            = [];

        public IReadOnlyList<KnowledgeRelationship> Relationships { get; set; }
            = [];
    }
}
