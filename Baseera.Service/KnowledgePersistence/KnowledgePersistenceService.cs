using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgePersistence.Abstracts;
using Baseera.Core.KnowledgeResolution.Abstracts;
using Baseera.Core.KnowledgeResolution.Enums;
using Baseera.Core.KnowledgeResolution.Models;
using Baseera.Domain.Entities;
using Baseera.Domain.Enums;

namespace Baseera.Service.KnowledgePersistence;

public sealed class KnowledgePersistenceService
    : IKnowledgePersistenceService
{
    private readonly IKnowledgeRepository _knowledgeRepository;
    private readonly IEmbeddingGenerator _embeddingGenerator;

    public KnowledgePersistenceService(
        IKnowledgeRepository knowledgeRepository,
        IEmbeddingGenerator embeddingGenerator)
    {
        _knowledgeRepository = knowledgeRepository;
        _embeddingGenerator = embeddingGenerator;
    }

    public async Task PersistAsync(
        KnowledgeExtractionResult extractedKnowledge,
        ResolvedKnowledge resolvedKnowledge,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(extractedKnowledge);
        ArgumentNullException.ThrowIfNull(resolvedKnowledge);

        var extractedEntities = extractedKnowledge.Entities
            .ToDictionary(e => e.Id);

        await PersistEntitiesAsync(
            extractedEntities,
            resolvedKnowledge,
            cancellationToken);

        await PersistAliasesAsync(
            extractedEntities,
            resolvedKnowledge,
            cancellationToken);

        await PersistRelationshipsAsync(
            resolvedKnowledge,
            cancellationToken);
    }

    private async Task PersistEntitiesAsync(
        IReadOnlyDictionary<string, ExtractedEntity> extractedEntities,
        ResolvedKnowledge resolvedKnowledge,
        CancellationToken cancellationToken)
    {
        foreach (var resolvedEntity in resolvedKnowledge.Entities)
        {
            if (resolvedEntity.Status != ResolutionStatus.New)
            {
                continue;
            }

            var extractedEntity =
                extractedEntities[resolvedEntity.ExtractedEntityId];

            var entity = await CreateEntityAsync(
                extractedEntity,
                resolvedEntity,
                cancellationToken);

            await _knowledgeRepository.AddEntityAsync(
                entity,
                cancellationToken);
        }
    }

    private async Task<BaseEntity> CreateEntityAsync(
        ExtractedEntity extractedEntity,
        ResolvedEntity resolvedEntity,
        CancellationToken cancellationToken)
    {
        if (resolvedEntity.ResolvedEntityId is null)
        {
            throw new InvalidOperationException(
                "A new entity must have a resolved entity ID.");
        }

        var embeddingText =
            BuildEmbeddingText(extractedEntity);

        var embedding =
            await _embeddingGenerator.GenerateAsync(
                embeddingText,
                cancellationToken);

        return CreateDomainEntity(
            extractedEntity,
            resolvedEntity.ResolvedEntityId.Value,
            embedding);
    }

    private async Task PersistAliasesAsync(
        IReadOnlyDictionary<string, ExtractedEntity> extractedEntities,
        ResolvedKnowledge resolvedKnowledge,
        CancellationToken cancellationToken)
    {
        foreach (var resolvedEntity in resolvedKnowledge.Entities)
        {
            if (resolvedEntity.Status != ResolutionStatus.Resolved ||
                resolvedEntity.ResolutionType != ResolutionType.AliasMatch ||
                resolvedEntity.ResolutionSource != ResolutionSource.Gemini)
            {
                continue;
            }

            if (resolvedEntity.ResolvedEntityId is null)
            {
                throw new InvalidOperationException(
                    "A Gemini alias match must have a resolved entity ID.");
            }

            var extractedEntity =
                extractedEntities[resolvedEntity.ExtractedEntityId];

            var alias = new Alias
            {
                Id = Guid.NewGuid(),
                EntityId = resolvedEntity.ResolvedEntityId.Value,
                Value = extractedEntity.Name,
                NormalizedValue = Normalize(extractedEntity.Name),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _knowledgeRepository.AddAliasAsync(
                alias,
                cancellationToken);
        }
    }

    private async Task PersistRelationshipsAsync(
        ResolvedKnowledge resolvedKnowledge,
        CancellationToken cancellationToken)
    {
        foreach (var resolvedRelationship
            in resolvedKnowledge.Relationships)
        {
            if (resolvedRelationship.Relationship
                == RelationshipType.HasAlias)
            {
                continue;
            }

            var relationship = new KnowledgeRelationship
            {
                Id = Guid.NewGuid(),
                SourceEntityId =
                    resolvedRelationship.SourceEntityId,
                TargetEntityId =
                    resolvedRelationship.TargetEntityId,
                Type = resolvedRelationship.Relationship,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _knowledgeRepository.AddRelationshipAsync(
                relationship,
                cancellationToken);
        }
    }

    private static string BuildEmbeddingText(
        ExtractedEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Description))
        {
            return entity.Name;
        }

        return $"{entity.Name}. {entity.Description}";
    }

    private static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant();
    }


    private static BaseEntity CreateDomainEntity(
    ExtractedEntity extractedEntity,
    Guid id,
    IReadOnlyList<double> embedding)
    {
        return extractedEntity.Type switch
        {
            EntityType.Brand =>
                new Brand
                {
                    Id = id,
                    Name = extractedEntity.Name,
                    NormalizedName = Normalize(extractedEntity.Name),
                    Description = extractedEntity.Description,
                    Embedding = embedding,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

            EntityType.Product =>
                new Product
                {
                    Id = id,
                    Name = extractedEntity.Name,
                    NormalizedName = Normalize(extractedEntity.Name),
                    Description = extractedEntity.Description,
                    Embedding = embedding,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,

                    ProductType = ProductType.Physical
                },

            EntityType.Service =>
                new Domain.Entities.Service
                {
                    Id = id,
                    Name = extractedEntity.Name,
                    NormalizedName = Normalize(extractedEntity.Name),
                    Description = extractedEntity.Description,
                    Embedding = embedding,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

            EntityType.Feature =>
                new Feature
                {
                    Id = id,
                    Name = extractedEntity.Name,
                    NormalizedName = Normalize(extractedEntity.Name),
                    Description = extractedEntity.Description,
                    Embedding = embedding,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

            EntityType.Topic =>
                new Topic
                {
                    Id = id,
                    Name = extractedEntity.Name,
                    NormalizedName = Normalize(extractedEntity.Name),
                    Description = extractedEntity.Description,
                    Embedding = embedding,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

            EntityType.Person =>
                new Person
                {
                    Id = id,
                    Name = extractedEntity.Name,
                    NormalizedName = Normalize(extractedEntity.Name),
                    Description = extractedEntity.Description,
                    Embedding = embedding,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

            EntityType.Campaign =>
                new Campaign
                {
                    Id = id,
                    Name = extractedEntity.Name,
                    NormalizedName = Normalize(extractedEntity.Name),
                    Description = extractedEntity.Description,
                    Embedding = embedding,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

            EntityType.Event =>
                new Event
                {
                    Id = id,
                    Name = extractedEntity.Name,
                    NormalizedName = Normalize(extractedEntity.Name),
                    Description = extractedEntity.Description,
                    Embedding = embedding,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

            EntityType.Alias =>
                throw new InvalidOperationException(
                    "Alias cannot be persisted as a canonical entity."),

            _ => throw new InvalidOperationException(
                $"Unsupported entity type: {extractedEntity.Type}")
        };
    }
}