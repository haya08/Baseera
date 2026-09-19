using Baseera.Domain.Entities;
using Baseera.Domain.Enums;
using Baseera.Infrastructure.KnowledgePersistence.Neo4j;
using Baseera.Infrastructure.Options;
using Baseera.Infrastructure.Tests.TestSetups;
using Neo4j.Driver;
using Xunit;

namespace Baseera.Infrastructure.Tests.KnowledgePersistence
{
    public class Neo4jAddRelationshipTest
    {
        [Fact]
        public async Task AddRelationshipAsync_CreatesRelationshipBetweenEntities()
        {
            TestEnvironment.Load();

            var neo4jDatabase = Environment.GetEnvironmentVariable("NEO4J_DATABASE");

            var driver = await CreateNeo4jDriver.CreateDriverAsync();

            var options =
                Microsoft.Extensions.Options.Options.Create(
                    new Neo4jOptions
                    {
                        Database = neo4jDatabase ?? "neo4j"
                    });

            var repository = new Neo4jKnowledgeRepository(
                driver,
                options);

            var brandId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var brand = new Brand
            {
                Id = brandId,
                Name = "Puma",
                NormalizedName = "puma",
                Embedding = Enumerable
                    .Repeat(0.1, 768)
                    .ToArray(),
                Description = "Sportswear brand",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var product = new Product
            {
                Id = productId,
                Name = "Puma Nitro",
                NormalizedName = "puma nitro",
                Embedding = Enumerable
                    .Repeat(0.2, 768)
                    .ToArray(),
                Description = "Running shoe",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await repository.AddEntityAsync(brand);
            await repository.AddEntityAsync(product);

            var relationship = new KnowledgeRelationship
            {
                Id = Guid.NewGuid(),
                SourceEntityId = brandId,
                TargetEntityId = productId,
                Type = RelationshipType.HasProduct,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await repository.AddRelationshipAsync(relationship);

            await using var session = driver.AsyncSession(
                o => o.WithDatabase(neo4jDatabase ?? "neo4j"));

            var result = await session.ExecuteReadAsync(
                async tx =>
                {
                    var cursor = await tx.RunAsync(
                        """
                        MATCH (source:Entity {id: $sourceId})
                              -[r:HAS_PRODUCT]->
                              (target:Entity {id: $targetId})
                        RETURN
                            source.name AS sourceName,
                            target.name AS targetName,
                            r.id AS relationshipId
                        """,
                        new
                        {
                            sourceId = brandId.ToString(),
                            targetId = productId.ToString()
                        });

                    return await cursor.SingleAsync();
                });

            Assert.Equal("Puma", result["sourceName"].As<string>());
            Assert.Equal("Puma Nitro", result["targetName"].As<string>());
            Assert.Equal(
                relationship.Id.ToString(),
                result["relationshipId"].As<string>());
        }
    }
}
