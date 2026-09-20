using Baseera.Domain.Entities;

namespace Baseera.Core.KnowledgePersistence.Abstracts
{
    public interface IKnowledgeRepository
    {
        Task AddEntityAsync(
            BaseEntity entity,
            CancellationToken cancellationToken = default);

        Task AddAliasAsync(
            Alias alias,
            CancellationToken cancellationToken = default);

        Task AddRelationshipAsync(
            KnowledgeRelationship relationship,
            CancellationToken cancellationToken = default);
    }
}
