using Baseera.Infrastructure.KnowledgeResolution.Gemini;
using Baseera.Infrastructure.Tests.TestSetups;
using Google.GenAI;
using Neo4j.Driver;
using Xunit;

namespace Baseera.Infrastructure.Tests.KnowledgeResolution
{
    public class Neo4jEmbeddingIntegrationTests
    {
        [Fact]
        public async Task GenerateAndStoreEmbedding_ForNikePegasus_Stores768Dimensions()
        {
            TestEnvironment.Load();

            // Arrange
            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");

            Assert.False(
                string.IsNullOrWhiteSpace(apiKey),
                "GEMINI_API_KEY environment variable is missing.");

            var neo4jUri =
                Environment.GetEnvironmentVariable("NEO4J_URI");

            var neo4jUsername =
                Environment.GetEnvironmentVariable("NEO4J_USERNAME");

            var neo4jPassword =
                Environment.GetEnvironmentVariable("NEO4J_PASSWORD");

            var neo4jDatabase =
                Environment.GetEnvironmentVariable("NEO4J_DATABASE")
                ?? "neo4j";

            Assert.False(
                string.IsNullOrWhiteSpace(neo4jUri),
                "NEO4J_URI is missing.");

            Assert.False(
                string.IsNullOrWhiteSpace(neo4jUsername),
                "NEO4J_USERNAME is missing.");

            Assert.False(
                string.IsNullOrWhiteSpace(neo4jPassword),
                "NEO4J_PASSWORD is missing.");

            var client = new Client(
                apiKey: apiKey);

            var embeddingGenerator =
                new GeminiEmbeddingGenerator(client);

            await using var driver = GraphDatabase.Driver(
                neo4jUri,
                AuthTokens.Basic(
                    neo4jUsername,
                    neo4jPassword));

            // Act - get product
            await using var session = driver.AsyncSession(
                options => options.WithDatabase(neo4jDatabase));

            var productId = "ef6c8c2c-dca3-4e4e-904e-c549c53ad4f4";

            var productResult = await session.RunAsync(
                """
                MATCH (p:Product {id: $id})
                RETURN p.name AS name,
                       p.description AS description
                LIMIT 1
                """,
                new Dictionary<string, object>
                {
                    ["id"] = productId
                });

            var record = await productResult.SingleAsync();

            var name = record["name"].As<string>();
            var description = record["description"].As<string>();

            var text = $"""
            Name: {name}
            Description: {description}
            """;

            // Generate real Gemini embedding
            var embedding =
                await embeddingGenerator.GenerateAsync(text);

            // Store embedding in Neo4j
            await session.RunAsync(
                """
                MATCH (p:Product {id: $id})
                SET p.embedding = $embedding
                """,
                new Dictionary<string, object>
                {
                    ["id"] = productId,
                    ["embedding"] = embedding
                });

            // Verify
            var verifyResult = await session.RunAsync(
                """
                MATCH (p:Product {id: $id})
                RETURN size(p.embedding) AS dimensions
                """,
                new Dictionary<string, object>
                {
                    ["id"] = productId
                });

            var verifyRecord = await verifyResult.SingleAsync();

            var dimensions =
                verifyRecord["dimensions"].As<int>();

            // Assert
            Assert.Equal(768, dimensions);
        }
    }
}
