using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Abstracts;
using Baseera.Core.KnowledgeResolution.Enums;
using Baseera.Core.KnowledgeResolution.Models;
using Baseera.Domain.Enums;

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
                return CreateNewEntity(entity);
            }

            var exactCandidate = candidates
                .FirstOrDefault(c =>
                    c.MatchType == CandidateMatchType.Exact);

            if (exactCandidate is not null)
            {
                return CreateResolvedEntity(
                    entity,
                    exactCandidate,
                    candidates,
                    ResolutionType.ExactMatch,
                    ResolutionSource.ExactMatch);
            }

            var aliasCandidate = candidates
                .FirstOrDefault(c =>
                    c.MatchType == CandidateMatchType.Alias);

            if (aliasCandidate is not null)
            {
                return CreateResolvedEntity(
                    entity,
                    aliasCandidate,
                    candidates,
                    ResolutionType.AliasMatch,
                    ResolutionSource.ExistingAlias);
            }

            var decision = await _resolutionJudge.JudgeAsync(
                entity,
                candidates,
                cancellationToken);

            return decision.Decision switch
            {
                ResolutionDecisionType.Match =>
                    CreateResolvedEntityFromDecision(
                        entity,
                        decision,
                        candidates),

                ResolutionDecisionType.NoMatch =>
                    CreateNewEntity(entity),

                ResolutionDecisionType.Ambiguous =>
                    CreateAmbiguousEntity(
                        entity,
                        candidates),

                _ => throw new InvalidOperationException(
                    $"Unsupported resolution decision: {decision.Decision}")
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
                    (sourceType == EntityType.Brand &&
                    targetType == EntityType.Topic) ||
                    (sourceType == EntityType.Product &&
                    targetType == EntityType.Topic),

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


        private static ResolvedEntity CreateNewEntity(
            ExtractedEntity entity)
        {
            return new ResolvedEntity
            {
                ExtractedEntityId = entity.Id,

                ResolvedEntityId = Guid.NewGuid(),

                Type = entity.Type,

                Status = ResolutionStatus.New,

                ResolutionType = null,

                ResolutionSource = null,

                Candidates = []
            };
        }


        private static ResolvedEntity CreateResolvedEntity(
            ExtractedEntity entity,
            ResolutionCandidate candidate,
            IReadOnlyList<ResolutionCandidate> candidates,
            ResolutionType resolutionType,
            ResolutionSource resolutionSource)
        {
            return new ResolvedEntity
            {
                ExtractedEntityId = entity.Id,

                ResolvedEntityId = candidate.EntityId,

                Type = entity.Type,

                Status = ResolutionStatus.Resolved,

                ResolutionType = resolutionType,

                ResolutionSource = resolutionSource,

                Candidates = candidates
            };
        }


        private static ResolvedEntity CreateResolvedEntityFromDecision(
            ExtractedEntity entity,
            ResolutionDecision decision,
            IReadOnlyList<ResolutionCandidate> candidates)
        {
            if (decision.CandidateEntityId is null)
            {
                throw new InvalidOperationException(
                    "Match decision must contain a candidate ID.");
            }

            if (decision.ResolutionType is null)
            {
                throw new InvalidOperationException(
                    "Match decision must contain a resolution type.");
            }

            var candidate = candidates.FirstOrDefault(
                c => c.EntityId == decision.CandidateEntityId.Value);

            if (candidate is null)
            {
                throw new InvalidOperationException(
                    "Selected candidate was not found.");
            }

            return CreateResolvedEntity(
                entity,
                candidate,
                candidates,
                decision.ResolutionType.Value,
                ResolutionSource.Gemini);
        }


        private static ResolvedEntity CreateAmbiguousEntity(
            ExtractedEntity entity,
            IReadOnlyList<ResolutionCandidate> candidates)
        {
            return new ResolvedEntity
            {
                ExtractedEntityId = entity.Id,

                ResolvedEntityId = null,

                Type = entity.Type,

                Status = ResolutionStatus.Ambiguous,

                ResolutionType = null,

                ResolutionSource = ResolutionSource.Gemini,

                Candidates = candidates
            };
        }


    }
}
