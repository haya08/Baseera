using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Abstracts;
using Baseera.Core.KnowledgeResolution.Models;

namespace Baseera.Infrastructure.KnowledgeResolution.Gemini;

public class GeminiResolutionJudge : IResolutionJudge
{
    public Task<ResolutionCandidate?> JudgeAsync(
        ExtractedEntity entity,
        IReadOnlyList<ResolutionCandidate> candidates,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ResolutionCandidate?>(null);
    }
}