using Baseera.Domain.Entities;
using Baseera.Infrastructure.KnowledgePersistence.Neo4j;
using Baseera.Infrastructure.Options;
using Baseera.Infrastructure.Tests.TestSetups;
using Neo4j.Driver;
using Xunit;

namespace Baseera.Infrastructure.Tests.KnowledgePersistence
{
    public class Neo4jAddAliasTest
    {
        [Fact]
        public async Task AddAliasAsync_CreatesAliasAndConnectsItToEntity()
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

            var productId = Guid.NewGuid();
            var aliasId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Name = "Nike Pegasus",
                NormalizedName = "nike pegasus",
                Embedding = Enumerable
                    .Repeat(0.1, 768)
                    .ToArray(),
                Description = "Running shoe",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await repository.AddEntityAsync(product);

            var alias = new Alias
            {
                Id = aliasId,
                EntityId = productId,
                Value = "Pegasus",
                NormalizedValue = "pegasus",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await repository.AddAliasAsync(alias);

            await using var session = driver.AsyncSession(
                o => o.WithDatabase(neo4jDatabase ?? "neo4j"));

            var result = await session.ExecuteReadAsync(
                async tx =>
                {
                    var cursor = await tx.RunAsync(
                        """
                        MATCH (e:Entity {id: $entityId})
                              -[:HAS_ALIAS]->
                              (a:Alias {id: $aliasId})
                        RETURN
                            e.name AS entityName,
                            a.id AS aliasId,
                            a.value AS value,
                            a.normalizedValue AS normalizedValue
                        """,
                        new
                        {
                            entityId = productId.ToString(),
                            aliasId = aliasId.ToString()
                        });

                    return await cursor.SingleAsync();
                });

            Assert.Equal("Nike Pegasus", result["entityName"].As<string>());
            Assert.Equal(aliasId.ToString(), result["aliasId"].As<string>());
            Assert.Equal("Pegasus", result["value"].As<string>());
            Assert.Equal("pegasus", result["normalizedValue"].As<string>());
        }
    }
}
