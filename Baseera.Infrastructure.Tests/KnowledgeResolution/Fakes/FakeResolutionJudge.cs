using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Abstracts;
using Baseera.Core.KnowledgeResolution.Models;

namespace Baseera.Infrastructure.Tests.KnowledgeResolution.Fakes
{
    public sealed class FakeResolutionJudge : IResolutionJudge
    {
        private readonly ResolutionDecision _decision;

        public int CallCount { get; private set; }

        public FakeResolutionJudge(
            ResolutionDecision decision)
        {
            _decision = decision;
        }

        public Task<ResolutionDecision> JudgeAsync(
            ExtractedEntity entity,
            IReadOnlyList<ResolutionCandidate> candidates,
            CancellationToken cancellationToken = default)
        {
            CallCount++;

            return Task.FromResult(_decision);
        }
    }
}
