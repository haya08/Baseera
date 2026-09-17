using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Abstracts;
using Baseera.Core.KnowledgeResolution.Models;

namespace Baseera.Infrastructure.Tests.KnowledgeResolution.Fakes
{
    public sealed class FakeCandidateProvider : ICandidateProvider
    {
        private readonly IReadOnlyList<ResolutionCandidate> _candidates;

        public FakeCandidateProvider(
            IReadOnlyList<ResolutionCandidate> candidates)
        {
            _candidates = candidates;
        }

        public Task<IReadOnlyList<ResolutionCandidate>> GetCandidatesAsync(
            ExtractedEntity entity,
            int topK,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_candidates);
        }
    }
}
