using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Enums;
using Baseera.Core.KnowledgeResolution.Models;
using Baseera.Domain.Enums;
using Baseera.Infrastructure.Tests.KnowledgePersistence.Fakes;
using Baseera.Service.KnowledgePersistence;
using Xunit;

namespace Baseera.Infrastructure.Tests.KnowledgePersistence;

public sealed class KnowledgePersistenceServiceTests
{
    [Fact]
    public async Task PersistAsync_NewEntity_AddsEntityWithSameResolvedId()
    {
        // Arrange
        var repository = new FakeKnowledgeRepository();
        var embeddingGenerator = new FakeEmbeddingGenerator();

        var service = new KnowledgePersistenceService(
            repository,
            embeddingGenerator);

        var entityId = Guid.NewGuid();

        var extractedEntity = new ExtractedEntity
        {
            Id = "brand_nike",
            Name = "Nike",
            Type = EntityType.Brand,
            Description = "Sportswear brand.",
            Confidence = 0.95
        };

        var extractionResult = new KnowledgeExtractionResult
        {
            Entities = [extractedEntity],
            Relationships = []
        };

        var resolvedEntity = new ResolvedEntity
        {
            ExtractedEntityId = "brand_nike",
            ResolvedEntityId = entityId,
            Type = EntityType.Brand,
            Status = ResolutionStatus.New,
            ResolutionType = null,
            ResolutionSource = null,
            Candidates = []
        };

        var resolvedKnowledge = new ResolvedKnowledge
        {
            Entities = [resolvedEntity],
            Relationships = []
        };

        // Act
        await service.PersistAsync(
            extractionResult,
            resolvedKnowledge);

        // Assert
        Assert.Single(repository.AddedEntities);

        var addedEntity = repository.AddedEntities[0];

        Assert.Equal(entityId, addedEntity.Id);
        Assert.Equal("Nike", addedEntity.Name);
        Assert.Equal(EntityType.Brand.ToString(), addedEntity.GetType().Name);

        Assert.Single(embeddingGenerator.GeneratedTexts);

        Assert.Equal(
            "Nike. Sportswear brand.",
            embeddingGenerator.GeneratedTexts[0]);
    }

    [Fact]
    public async Task PersistAsync_ExactMatch_DoesNotAddEntity()
    {
        // Arrange
        var repository = new FakeKnowledgeRepository();
        var embeddingGenerator = new FakeEmbeddingGenerator();

        var service = new KnowledgePersistenceService(
            repository,
            embeddingGenerator);

        var existingEntityId = Guid.NewGuid();

        var extractedEntity = new ExtractedEntity
        {
            Id = "product_pegasus",
            Name = "Nike Pegasus",
            Type = EntityType.Product,
            Description = "Running shoe",
            Confidence = 0.95
        };

        var extractionResult = new KnowledgeExtractionResult
        {
            Entities = [extractedEntity],
            Relationships = []
        };

        var resolvedEntity = new ResolvedEntity
        {
            ExtractedEntityId = "product_pegasus",
            ResolvedEntityId = existingEntityId,
            Type = EntityType.Product,
            Status = ResolutionStatus.Resolved,
            ResolutionType = ResolutionType.ExactMatch,
            ResolutionSource = ResolutionSource.ExactMatch,
            Candidates = []
        };

        var resolvedKnowledge = new ResolvedKnowledge
        {
            Entities = [resolvedEntity],
            Relationships = []
        };

        // Act
        await service.PersistAsync(
            extractionResult,
            resolvedKnowledge);

        // Assert
        Assert.Empty(repository.AddedEntities);
        Assert.Empty(embeddingGenerator.GeneratedTexts);
    }

    [Fact]
    public async Task PersistAsync_ExistingAlias_DoesNotAddAlias()
    {
        // Arrange
        var repository = new FakeKnowledgeRepository();
        var embeddingGenerator = new FakeEmbeddingGenerator();

        var service = new KnowledgePersistenceService(
            repository,
            embeddingGenerator);

        var existingEntityId = Guid.NewGuid();

        var extractedEntity = new ExtractedEntity
        {
            Id = "product_pegasus",
            Name = "Pegasus",
            Type = EntityType.Product,
            Description = "Running shoe",
            Confidence = 0.95
        };

        var extractionResult = new KnowledgeExtractionResult
        {
            Entities = [extractedEntity],
            Relationships = []
        };

        var resolvedEntity = new ResolvedEntity
        {
            ExtractedEntityId = "product_pegasus",
            ResolvedEntityId = existingEntityId,
            Type = EntityType.Product,
            Status = ResolutionStatus.Resolved,
            ResolutionType = ResolutionType.AliasMatch,
            ResolutionSource = ResolutionSource.ExistingAlias,
            Candidates = []
        };

        var resolvedKnowledge = new ResolvedKnowledge
        {
            Entities = [resolvedEntity],
            Relationships = []
        };

        // Act
        await service.PersistAsync(
            extractionResult,
            resolvedKnowledge);

        // Assert
        Assert.Empty(repository.AddedEntities);
        Assert.Empty(repository.AddedAliases);
        Assert.Empty(embeddingGenerator.GeneratedTexts);
    }

    [Fact]
    public async Task PersistAsync_GeminiAlias_AddsAlias()
    {
        // Arrange
        var repository = new FakeKnowledgeRepository();
        var embeddingGenerator = new FakeEmbeddingGenerator();

        var service = new KnowledgePersistenceService(
            repository,
            embeddingGenerator);

        var existingEntityId = Guid.NewGuid();

        var extractedEntity = new ExtractedEntity
        {
            Id = "brand_nike",
            Name = "Nike Inc.",
            Type = EntityType.Brand,
            Description = "Sportswear brand.",
            Confidence = 0.95
        };

        var extractionResult = new KnowledgeExtractionResult
        {
            Entities = [extractedEntity],
            Relationships = []
        };

        var resolvedEntity = new ResolvedEntity
        {
            ExtractedEntityId = "brand_nike",
            ResolvedEntityId = existingEntityId,
            Type = EntityType.Brand,
            Status = ResolutionStatus.Resolved,
            ResolutionType = ResolutionType.AliasMatch,
            ResolutionSource = ResolutionSource.Gemini,
            Candidates = []
        };

        var resolvedKnowledge = new ResolvedKnowledge
        {
            Entities = [resolvedEntity],
            Relationships = []
        };

        // Act
        await service.PersistAsync(
            extractionResult,
            resolvedKnowledge);

        // Assert
        Assert.Empty(repository.AddedEntities);

        Assert.Single(repository.AddedAliases);

        var addedAlias = repository.AddedAliases[0];

        Assert.Equal(existingEntityId, addedAlias.EntityId);
        Assert.Equal("Nike Inc.", addedAlias.Value);
        Assert.Equal("nike inc.", addedAlias.NormalizedValue);

        Assert.NotEqual(Guid.Empty, addedAlias.Id);
        Assert.NotEqual(default, addedAlias.CreatedAt);
        Assert.NotEqual(default, addedAlias.UpdatedAt);

        Assert.Empty(embeddingGenerator.GeneratedTexts);
    }

    [Fact]
    public async Task PersistAsync_AmbiguousEntity_DoesNotPersistAnything()
    {
        // Arrange
        var repository = new FakeKnowledgeRepository();
        var embeddingGenerator = new FakeEmbeddingGenerator();

        var service = new KnowledgePersistenceService(
            repository,
            embeddingGenerator);

        var extractedEntity = new ExtractedEntity
        {
            Id = "product_apple",
            Name = "Apple",
            Type = EntityType.Product,
            Description = "Product mentioned in text",
            Confidence = 0.95
        };

        var extractionResult = new KnowledgeExtractionResult
        {
            Entities = [extractedEntity],
            Relationships = []
        };

        var resolvedEntity = new ResolvedEntity
        {
            ExtractedEntityId = "product_apple",
            ResolvedEntityId = null,
            Type = EntityType.Product,
            Status = ResolutionStatus.Ambiguous,
            ResolutionType = null,
            ResolutionSource = ResolutionSource.Gemini,
            Candidates = []
        };

        var resolvedKnowledge = new ResolvedKnowledge
        {
            Entities = [resolvedEntity],
            Relationships = []
        };

        // Act
        await service.PersistAsync(
            extractionResult,
            resolvedKnowledge);

        // Assert
        Assert.Empty(repository.AddedEntities);
        Assert.Empty(repository.AddedAliases);
        Assert.Empty(repository.AddedRelationships);
        Assert.Empty(embeddingGenerator.GeneratedTexts);
    }

    [Fact]
    public async Task PersistAsync_Relationship_AddsRelationship()
    {
        // Arrange
        var repository = new FakeKnowledgeRepository();
        var embeddingGenerator = new FakeEmbeddingGenerator();

        var service = new KnowledgePersistenceService(
            repository,
            embeddingGenerator);

        var sourceEntityId = Guid.NewGuid();
        var targetEntityId = Guid.NewGuid();

        var extractionResult = new KnowledgeExtractionResult
        {
            Entities = [],
            Relationships = []
        };

        var resolvedRelationship = new ResolvedRelationship
        {
            ExtractedRelationshipId = "rel_1",
            SourceEntityId = sourceEntityId,
            Relationship = RelationshipType.HasProduct,
            TargetEntityId = targetEntityId
        };

        var resolvedKnowledge = new ResolvedKnowledge
        {
            Entities = [],
            Relationships = [resolvedRelationship]
        };

        // Act
        await service.PersistAsync(
            extractionResult,
            resolvedKnowledge);

        // Assert
        Assert.Single(repository.AddedRelationships);

        var addedRelationship = repository.AddedRelationships[0];

        Assert.NotEqual(Guid.Empty, addedRelationship.Id);

        Assert.Equal(
            sourceEntityId,
            addedRelationship.SourceEntityId);

        Assert.Equal(
            targetEntityId,
            addedRelationship.TargetEntityId);

        Assert.Equal(
            RelationshipType.HasProduct,
            addedRelationship.Type);

        Assert.NotEqual(default, addedRelationship.CreatedAt);
        Assert.NotEqual(default, addedRelationship.UpdatedAt);

        Assert.Empty(repository.AddedEntities);
        Assert.Empty(repository.AddedAliases);
        Assert.Empty(embeddingGenerator.GeneratedTexts);
    }

}
