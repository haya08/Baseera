using Baseera.Core.KnowledgeResolution.Enums;

namespace Baseera.Core.KnowledgeResolution.Models
{
    public sealed class ResolutionDecision
    {
        public ResolutionDecisionType Decision { get; init; }

        public Guid? CandidateEntityId { get; init; }

        public double Confidence { get; init; }

        public string? Reason { get; init; }
    }
}
