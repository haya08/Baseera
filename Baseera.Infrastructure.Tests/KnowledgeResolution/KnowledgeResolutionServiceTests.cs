using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Enums;
using Baseera.Core.KnowledgeResolution.Models;
using Baseera.Domain.Enums;
using Baseera.Infrastructure.Tests.KnowledgeResolution.Fakes;
using Baseera.Service.KnowledgeResolution;
using Xunit;

namespace Baseera.Infrastructure.Tests.KnowledgeResolution
{
    public class KnowledgeResolutionServiceTests
    {
        [Fact]
        public async Task ResolveAsync_WhenNoCandidates_ReturnsNewEntity()
        {
            // Arrange

            var candidateProvider =
                new FakeCandidateProvider([]);

            var judge =
                new FakeResolutionJudge(
                    new ResolutionDecision
                    {
                        Decision = ResolutionDecisionType.Match,
                        CandidateEntityId = Guid.NewGuid(),
                        Confidence = 1.0,
                        Reason = "Should not be called."
                    });

            var service =
                new KnowledgeResolutionService(
                    candidateProvider,
                    judge);

            var entity = new ExtractedEntity
            {
                Id = "e1",
                Name = "Nike",
                Type = EntityType.Brand,
                Description = "Sportswear brand.",
                Confidence = 0.95
            };

            var extractionResult =
                new KnowledgeExtractionResult
                {
                    Entities = [entity],
                    Relationships = []
                };

            // Act

            var result =
                await service.ResolveAsync(
                    extractionResult);

            // Assert

            var resolvedEntity =
                Assert.Single(result.Entities);

            Assert.Equal(
                ResolutionStatus.New,
                resolvedEntity.Status);

            Assert.NotNull(
                resolvedEntity.ResolvedEntityId);

            Assert.Equal(
                entity.Id,
                resolvedEntity.ExtractedEntityId);

            Assert.Equal(
                EntityType.Brand,
                resolvedEntity.Type);

            Assert.Empty(
                resolvedEntity.Candidates);

            Assert.Empty(
                result.Relationships);
        }

        [Fact]
        public async Task ResolveAsync_WhenExactMatchExists_ReturnsResolvedEntityWithoutCallingJudge()
        {
            // Arrange

            var existingEntityId = Guid.NewGuid();

            var exactCandidate =
                new ResolutionCandidate
                {
                    EntityId = existingEntityId,
                    Name = "Nike",
                    Type = EntityType.Brand,
                    Description = "Sportswear brand.",
                    MatchType = CandidateMatchType.Exact,
                    TextScore = 1.0,
                    FinalScore = 1.0
                };

            var candidateProvider =
                new FakeCandidateProvider(
                    [exactCandidate]);

            var judge =
                new FakeResolutionJudge(
                    new ResolutionDecision
                    {
                        Decision = ResolutionDecisionType.Ambiguous,
                        Confidence = 0.5,
                        Reason = "Judge should not be called."
                    });

            var service =
                new KnowledgeResolutionService(
                    candidateProvider,
                    judge);

            var entity = new ExtractedEntity
            {
                Id = "e1",
                Name = "Nike",
                Type = EntityType.Brand,
                Description = "Sportswear brand.",
                Confidence = 0.95
            };

            var extractionResult =
                new KnowledgeExtractionResult
                {
                    Entities = [entity],
                    Relationships = []
                };

            // Act

            var result =
                await service.ResolveAsync(
                    extractionResult);

            // Assert

            var resolvedEntity =
                Assert.Single(result.Entities);

            Assert.Equal(
                ResolutionStatus.Resolved,
                resolvedEntity.Status);

            Assert.Equal(
                existingEntityId,
                resolvedEntity.ResolvedEntityId);

            Assert.Equal(
                entity.Id,
                resolvedEntity.ExtractedEntityId);

            Assert.Equal(
                EntityType.Brand,
                resolvedEntity.Type);

            var candidate =
                Assert.Single(resolvedEntity.Candidates);

            Assert.Equal(
                existingEntityId,
                candidate.EntityId);

            Assert.Equal(
                CandidateMatchType.Exact,
                candidate.MatchType);

            Assert.Equal(
                0,
                judge.CallCount);

            Assert.Empty(
                result.Relationships);
        }
    }
}
