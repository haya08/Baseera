using Baseera.Core.Documents.Enums;
using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Abstracts;
using Baseera.Core.KnowledgeResolution.Enums;
using Baseera.Core.KnowledgeResolution.Models;

namespace Baseera.Service.KnowledgeResolution
{
    public sealed class KnowledgeResolutionService : IKnowledgeResolver
    {
        private readonly ICandidateProvider _candidateProvider;
        private readonly IResolutionJudge _resolutionJudge;

        public KnowledgeResolutionService(
            ICandidateProvider candidateProvider,
            IResolutionJudge resolutionJudge)
        {
            _candidateProvider = candidateProvider;
            _resolutionJudge = resolutionJudge;
        }

        public async Task<ResolvedKnowledge> ResolveAsync(
            KnowledgeExtractionResult extractionResult,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(extractionResult);

            var resolvedEntities = new List<ResolvedEntity>();

            foreach (var entity in extractionResult.Entities)
            {
                var resolvedEntity = await ResolveEntityAsync(
                    entity,
                    cancellationToken);

                resolvedEntities.Add(resolvedEntity);
            }

            var resolvedRelationships = ResolveRelationships(
                extractionResult.Relationships,
                resolvedEntities);

            return new ResolvedKnowledge
            {
                Entities = resolvedEntities,
                Relationships = resolvedRelationships
            };
        }


        private async Task<ResolvedEntity> ResolveEntityAsync(
            ExtractedEntity entity,
            CancellationToken cancellationToken)
        {
            var candidates = await _candidateProvider.GetCandidatesAsync(
                entity,
                topK: 5,
                cancellationToken);

            if (candidates.Count == 0)
            {
                return new ResolvedEntity
                {
                    ExtractedEntityId = entity.Id,
                    ResolvedEntityId = Guid.NewGuid(),
                    Type = entity.Type,
                    Status = ResolutionStatus.New,
                    Candidates = []
                };
            }

            var exactCandidate = candidates
                .FirstOrDefault(c =>
                    c.MatchType == CandidateMatchType.Exact);

            if (exactCandidate is not null)
            {
                return new ResolvedEntity
                {
                    ExtractedEntityId = entity.Id,
                    ResolvedEntityId = exactCandidate.EntityId,
                    Type = entity.Type,
                    Status = ResolutionStatus.Resolved,
                    Candidates = candidates
                };
            }

            var aliasCandidate = candidates
                .FirstOrDefault(c =>
                    c.MatchType == CandidateMatchType.Alias);

            if (aliasCandidate is not null)
            {
                return new ResolvedEntity
                {
                    ExtractedEntityId = entity.Id,
                    ResolvedEntityId = aliasCandidate.EntityId,
                    Type = entity.Type,
                    Status = ResolutionStatus.Resolved,
                    Candidates = candidates
                };
            }

            var judgedCandidate = await _resolutionJudge.JudgeAsync(
                entity,
                candidates,
                cancellationToken);

            if (judgedCandidate is not null)
            {
                return new ResolvedEntity
                {
                    ExtractedEntityId = entity.Id,
                    ResolvedEntityId = judgedCandidate.EntityId,
                    Type = entity.Type,
                    Status = ResolutionStatus.Resolved,
                    Candidates = candidates
                };
            }

            return new ResolvedEntity
            {
                ExtractedEntityId = entity.Id,
                ResolvedEntityId = null,
                Type = entity.Type,
                Status = ResolutionStatus.Ambiguous,
                Candidates = candidates
            };
        }


        private static IReadOnlyList<ResolvedRelationship> ResolveRelationships(
            IReadOnlyList<ExtractedRelationship> relationships,
            IReadOnlyList<ResolvedEntity> resolvedEntities)
        {
            var entityMap = resolvedEntities
                .ToDictionary(e => e.ExtractedEntityId);

            var resolvedRelationships =
                new List<ResolvedRelationship>();

            foreach (var relationship in relationships)
            {
                var resolvedRelationship = ResolveRelationship(
                    relationship,
                    entityMap);

                if (resolvedRelationship is not null)
                {
                    resolvedRelationships.Add(resolvedRelationship);
                }
            }

            return resolvedRelationships;
        }


        private static ResolvedRelationship? ResolveRelationship(
            ExtractedRelationship relationship,
            IReadOnlyDictionary<string, ResolvedEntity> entityMap)
        {
            if (!entityMap.TryGetValue(
                    relationship.SourceId,
                    out var source))
            {
                return null;
            }

            if (!entityMap.TryGetValue(
                    relationship.TargetId,
                    out var target))
            {
                return null;
            }

            if (source.ResolvedEntityId is null ||
                target.ResolvedEntityId is null)
            {
                return null;
            }

            if (source.Status == ResolutionStatus.Ambiguous ||
                target.Status == ResolutionStatus.Ambiguous)
            {
                return null;
            }

            if (!IsValidRelationship(
                    source.Type,
                    relationship.Relationship,
                    target.Type))
            {
                return null;
            }

            return new ResolvedRelationship
            {
                SourceEntityId = source.ResolvedEntityId.Value,
                Relationship = relationship.Relationship,
                TargetEntityId = target.ResolvedEntityId.Value
            };
        }


        private static bool IsValidRelationship(
            EntityType sourceType,
            RelationshipType relationship,
            EntityType targetType)
        {
            return relationship switch
            {
                RelationshipType.HasAlias =>
                    sourceType == EntityType.Brand &&
                    targetType == EntityType.Alias,

                RelationshipType.HasProduct =>
                    sourceType == EntityType.Brand &&
                    targetType == EntityType.Product,

                RelationshipType.HasService =>
                    sourceType == EntityType.Brand &&
                    targetType == EntityType.Service,

                RelationshipType.HasFeature =>
                    (sourceType == EntityType.Product ||
                     sourceType == EntityType.Service) &&
                    targetType == EntityType.Feature,

                RelationshipType.HasTopic =>
                    sourceType == EntityType.Brand &&
                    targetType == EntityType.Topic,

                RelationshipType.RunsCampaign =>
                    sourceType == EntityType.Brand &&
                    targetType == EntityType.Campaign,

                RelationshipType.HasEvent =>
                    sourceType == EntityType.Brand &&
                    targetType == EntityType.Event,

                RelationshipType.LedBy =>
                    sourceType == EntityType.Campaign &&
                    targetType == EntityType.Person,

                _ => false
            };
        }
    }
}
