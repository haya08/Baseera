using Baseera.Domain.Entities;
using Baseera.Infrastructure.KnowledgePersistence.Neo4j;
using Baseera.Infrastructure.Options;
using Baseera.Infrastructure.Tests.TestSetups;
using Neo4j.Driver;
using Xunit;

namespace Baseera.Infrastructure.Tests.KnowledgePersistence
{
    public class Neo4jAddEntityTest
    {
        [Fact]
        public async Task AddEntityAsync_CreatesProductNode()
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

            var product = new Product
            {
                Id = productId,
                Name = "Nike Air Max",
                NormalizedName = "nike air max",
                Embedding = Enumerable
                    .Repeat(0.1, 768)
                    .ToArray(),
                Description = "Running shoe",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await repository.AddEntityAsync(product);

            await using var session = driver.AsyncSession(
                o => o.WithDatabase(neo4jDatabase ?? "neo4j"));

            var result = await session.ExecuteReadAsync(
                async tx =>
                {
                    var cursor = await tx.RunAsync(
                        """
                        MATCH (e:Entity:Product {id: $id})
                        RETURN
                            e.id AS id,
                            e.name AS name,
                            e.normalizedName AS normalizedName,
                            e.description AS description,
                            size(e.embedding) AS embeddingSize
                        """,
                        new
                        {
                            id = productId.ToString()
                        });

                    return await cursor.SingleAsync();
                });

            Assert.Equal(productId.ToString(), result["id"].As<string>());
            Assert.Equal("Nike Air Max", result["name"].As<string>());
            Assert.Equal("nike air max", result["normalizedName"].As<string>());
            Assert.Equal("Running shoe", result["description"].As<string>());
            Assert.Equal(768, result["embeddingSize"].As<int>());
        }
    }
}
