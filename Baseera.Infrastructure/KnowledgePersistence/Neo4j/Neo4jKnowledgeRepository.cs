using Baseera.Core.KnowledgePersistence.Abstracts;
using Baseera.Domain.Entities;
using Baseera.Infrastructure.Helpers;
using Baseera.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace Baseera.Infrastructure.KnowledgePersistence.Neo4j
{
    public sealed class Neo4jKnowledgeRepository : IKnowledgeRepository
    {
        private readonly IDriver _driver;
        private readonly Neo4jOptions _options;

        public Neo4jKnowledgeRepository(
            IDriver driver,
            IOptions<Neo4jOptions> options)
        {
            _driver = driver;
            _options = options.Value;
        }

        public async Task AddEntityAsync(
            BaseEntity entity,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var label = Neo4jPersistenceHelpers.GetEntityLabel(entity);

            var query = $"""
            CREATE (e:Entity:{label})
            SET e.id = $id,
                e.name = $name,
                e.normalizedName = $normalizedName,
                e.embedding = $embedding,
                e.description = $description,
                e.createdAt = $createdAt,
                e.updatedAt = $updatedAt
            """;

            await using var session = _driver.AsyncSession(
                o => o.WithDatabase(_options.Database));

            await session.ExecuteWriteAsync(
                async tx =>
                {
                    await tx.RunAsync(
                        query,
                        new
                        {
                            id = entity.Id.ToString(),
                            name = entity.Name,
                            normalizedName = entity.NormalizedName,
                            embedding = entity.Embedding.ToArray(),
                            description = entity.Description,
                            createdAt = entity.CreatedAt,
                            updatedAt = entity.UpdatedAt
                        });

                    return true;
                });
        }


        public async Task AddAliasAsync(
            Alias alias,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(alias);

            const string query = """
                MATCH (e:Entity {id: $entityId})
                CREATE (e)-[:HAS_ALIAS]->(a:Alias)
                SET a.id = $id,
                    a.value = $value,
                    a.normalizedValue = $normalizedValue,
                    a.createdAt = $createdAt,
                    a.updatedAt = $updatedAt
                """;

            await using var session = _driver.AsyncSession(
                o => o.WithDatabase(_options.Database));

            await session.ExecuteWriteAsync(
                async tx =>
                {
                    await tx.RunAsync(
                        query,
                        new
                        {
                            id = alias.Id.ToString(),
                            entityId = alias.EntityId.ToString(),
                            value = alias.Value,
                            normalizedValue = alias.NormalizedValue,
                            createdAt = alias.CreatedAt,
                            updatedAt = alias.UpdatedAt
                        });

                    return true;
                });
        }


        public async Task AddRelationshipAsync(
            KnowledgeRelationship relationship,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(relationship);

            var relationshipType = Neo4jPersistenceHelpers.GetRelationshipType(relationship.Type);

            var query = $$$"""
                MATCH (source:Entity {id: $sourceId})
                MATCH (target:Entity {id: $targetId})

                MERGE (source)-[r:{{{relationshipType}}}]->(target)

                SET r.id = $id,
                    r.createdAt = $createdAt,
                    r.updatedAt = $updatedAt
                """;

            await using var session = _driver.AsyncSession(
                o => o.WithDatabase(_options.Database));

            await session.ExecuteWriteAsync(
                async tx =>
                {
                    await tx.RunAsync(
                        query,
                        new
                        {
                            id = relationship.Id.ToString(),
                            sourceId = relationship.SourceEntityId.ToString(),
                            targetId = relationship.TargetEntityId.ToString(),
                            createdAt = relationship.CreatedAt,
                            updatedAt = relationship.UpdatedAt
                        });

                    return true;
                });
        }
    }
}
