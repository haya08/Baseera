using Baseera.Core.KnowledgeResolution.Enums;
using Baseera.Domain.Enums;

namespace Baseera.Core.KnowledgeResolution.Models
{
    public sealed class ResolvedEntity
    {
        public string ExtractedEntityId { get; init; } = null!;

        public Guid? ResolvedEntityId { get; init; }

        public EntityType Type { get; init; }

        public ResolutionStatus Status { get; init; }

        public ResolutionType? ResolutionType { get; init; }

        public ResolutionSource? ResolutionSource { get; init; }

        public IReadOnlyList<ResolutionCandidate> Candidates { get; init; }
            = [];
    }
}
