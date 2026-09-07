using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Models;

namespace Baseera.Core.KnowledgeResolution.Abstracts
{
    public interface ICandidateProvider
    {
        Task<IReadOnlyList<ResolutionCandidate>> GetCandidatesAsync(
            ExtractedEntity entity,
            int topK,
            CancellationToken cancellationToken = default);
    }
}