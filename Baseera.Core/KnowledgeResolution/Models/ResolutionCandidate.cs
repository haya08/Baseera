using Baseera.Core.Documents.Enums;
using Baseera.Core.KnowledgeResolution.Enums;

namespace Baseera.Core.KnowledgeResolution.Models
{
    public class ResolutionCandidate
    {
        public Guid EntityId { get; set; }

        public string Name { get; set; } = null!;

        public EntityType Type { get; set; }

        public string? Description { get; set; }

        public double? TextScore { get; set; }

        public double? VectorScore { get; set; }

        public double? FinalScore { get; set; }

        public CandidateMatchType MatchType { get; set; }
    }
}
