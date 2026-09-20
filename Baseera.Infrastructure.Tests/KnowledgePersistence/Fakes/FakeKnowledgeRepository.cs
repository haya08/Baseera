using Baseera.Core.KnowledgePersistence.Abstracts;
using Baseera.Domain.Entities;

namespace Baseera.Infrastructure.Tests.KnowledgePersistence.Fakes;

internal sealed class FakeKnowledgeRepository : IKnowledgeRepository
{
    public List<BaseEntity> AddedEntities { get; } = [];

    public List<Alias> AddedAliases { get; } = [];

    public List<KnowledgeRelationship> AddedRelationships { get; } = [];

    public Task AddEntityAsync(
        BaseEntity entity,
        CancellationToken cancellationToken = default)
    {
        AddedEntities.Add(entity);

        return Task.CompletedTask;
    }

    public Task AddAliasAsync(
        Alias alias,
        CancellationToken cancellationToken = default)
    {
        AddedAliases.Add(alias);

        return Task.CompletedTask;
    }

    public Task AddRelationshipAsync(
        KnowledgeRelationship relationship,
        CancellationToken cancellationToken = default)
    {
        AddedRelationships.Add(relationship);

        return Task.CompletedTask;
    }
}