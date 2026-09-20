using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Models;

namespace Baseera.Core.KnowledgeResolution.Abstracts
{
    public interface IResolutionJudge
    {
        Task<ResolutionDecision> JudgeAsync(
            ExtractedEntity entity,
            IReadOnlyList<ResolutionCandidate> candidates,
            CancellationToken cancellationToken = default);
    }
}