using Baseera.Core.Documents.Enums;
using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Enums;
using Baseera.Infrastructure.KnowledgeResolution.Gemini;
using Baseera.Infrastructure.KnowledgeResolution.Neo4j;
using Baseera.Infrastructure.Options;
using Baseera.Infrastructure.Tests.TestSetups;
using Google.GenAI;

//using Baseera.Infrastructure.Tests.KnowledgeResolution.Fakes;
using Neo4j.Driver;
using Xunit;

namespace Baseera.Infrastructure.Tests.KnowledgeResolution
{
    public class Neo4jCandidateProviderTests : IAsyncLifetime
    {
        private IDriver _driver = null!;
        private Neo4jCandidateProvider _provider = null!;

        public async Task InitializeAsync()
        {
            TestEnvironment.Load();

            var uri = Environment.GetEnvironmentVariable("NEO4J_URI");
            var username = Environment.GetEnvironmentVariable("NEO4J_USERNAME");
            var password = Environment.GetEnvironmentVariable("NEO4J_PASSWORD");
            var database = Environment.GetEnvironmentVariable("NEO4J_DATABASE");

            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");

            Assert.False(
                string.IsNullOrWhiteSpace(apiKey),
                "GEMINI_API_KEY environment variable is missing.");

            if (string.IsNullOrWhiteSpace(uri) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Neo4j connection settings are missing.");
            }

            _driver = GraphDatabase.Driver(
                uri,
                AuthTokens.Basic(username, password));

            var options = Microsoft.Extensions.Options.Options.Create(
                new Neo4jOptions
                {
                    Database = database ?? "neo4j"
                });

            // Temporary test embedding generator.
            var client = new Client(
                apiKey: apiKey);

            var embeddingGenerator =
                new GeminiEmbeddingGenerator(client);

            _provider = new Neo4jCandidateProvider(
                _driver,
                options,
                embeddingGenerator);
        }

        public async Task DisposeAsync()
        {
            await _driver.DisposeAsync();
        }

        [Fact]
        public async Task GetCandidatesAsync_WhenEntityExists_ReturnsExactMatch()
        {
            // Arrange
            var entity = new ExtractedEntity
            {
                Id = "e1",
                Name = "Nike",
                Type = EntityType.Brand,
                Description = "Sportswear brand"
            };

            // Act
            var candidates = await _provider.GetCandidatesAsync(
                entity,
                topK: 5);

            // Assert
            Assert.NotEmpty(candidates);

            var candidate = Assert.Single(candidates);

            Assert.Equal(
                "1b54ba3e-91e9-4728-8b68-96ae4936090b",
                candidate.EntityId.ToString());

            Assert.Equal(
                "Nike",
                candidate.Name);

            Assert.Equal(
                EntityType.Brand,
                candidate.Type);

            Assert.Equal(
                CandidateMatchType.Exact,
                candidate.MatchType);
        }

        [Fact]
        public async Task GetCandidatesAsync_WhenAliasExists_ReturnsAliasMatch()
        {
            // Arrange
            var entity = new ExtractedEntity
            {
                Id = "e2",
                Name = "Nike Inc",
                Type = EntityType.Brand,
                Description = "Sportswear brand"
            };

            // Act
            var candidates = await _provider.GetCandidatesAsync(
                entity,
                topK: 5);

            // Assert
            Assert.NotEmpty(candidates);

            var candidate = Assert.Single(candidates);

            Assert.Equal(
                "1b54ba3e-91e9-4728-8b68-96ae4936090b",
                candidate.EntityId.ToString());

            Assert.Equal(
                "Nike",
                candidate.Name);

            Assert.Equal(
                EntityType.Brand,
                candidate.Type);

            Assert.Equal(
                CandidateMatchType.Alias,
                candidate.MatchType);
        }

        [Fact]
        public async Task GetCandidatesAsync_WhenVectorMatches_ReturnsVectorCandidate()
        {
            // Arrange
            const string testId = "e8800d47-d795-4305-9d0e-648e750aa87f";

            var embedding = new double[768];
            embedding[0] = 1.0;

            await using (var session = _driver.AsyncSession(
                o => o.WithDatabase("baseera")))
            {
                await session.RunAsync(
                    """
                    MERGE (p:Product {
                        id:$id
                    })
                    SET p.name = 'Vector Test Product',
                        p.description = 'A product created for vector search testing',
                        p.normalizedName = 'vector test product',
                        p.embedding = $embedding
                    """,
                    new Dictionary<string, object>
                    {
                        ["id"] = testId,
                        ["embedding"] = embedding
                    });
            }

            var entity = new ExtractedEntity
            {
                Id = "e-vector-test",
                Name = "Completely Unrelated XYZ Product",
                Type = EntityType.Product,
                Description = "This text should not match the test product lexically."
            };

            try
            {
                // Act
                var candidates = await _provider.GetCandidatesAsync(
                    entity,
                    topK: 5);

                // Assert
                Assert.NotEmpty(candidates);

                var candidate = candidates
                    .FirstOrDefault(x => x.EntityId.ToString() == testId);

                Assert.NotNull(candidate);

                Assert.Equal(
                    "Vector Test Product",
                    candidate.Name);

                Assert.Equal(
                    EntityType.Product,
                    candidate.Type);

                Assert.True(
                    candidate.VectorScore.HasValue,
                    "Expected a vector score.");

                Assert.Equal(
                    CandidateMatchType.Vector,
                    candidate.MatchType);
            }
            finally
            {
                // Cleanup test data
                await using var session = _driver.AsyncSession(
                    o => o.WithDatabase("baseera"));

                await session.RunAsync(
                    """
                    MATCH (p:Product {id: $id})
                    DETACH DELETE p
                    """,
                    new Dictionary<string, object>
                    {
                        ["id"] = testId
                    });
            }
        }

        [Fact]
        public async Task GetCandidatesAsync_WithRealEmbedding_ReturnsSemanticallySimilarProduct()
        {
            // Arrange
            var entity = new ExtractedEntity
            {
                Id = "e-real-vector",
                Name = "Nike running footwear",
                Type = EntityType.Product,
                Description = "Footwear designed for running"
            };

            // Act
            var candidates = await _provider.GetCandidatesAsync(
                entity,
                topK: 5);

            // Assert
            Assert.NotEmpty(candidates);

            var candidate = candidates
                .FirstOrDefault(x => x.EntityId.ToString() == "ef6c8c2c-dca3-4e4e-904e-c549c53ad4f4");

            Assert.NotNull(candidate);

            Assert.Equal(
                "Nike Pegasus",
                candidate.Name);

            Assert.Equal(
                EntityType.Product,
                candidate.Type);

            Assert.True(
                candidate.VectorScore.HasValue,
                "Expected a vector score.");

            Assert.True(
                candidate.VectorScore.Value > 0,
                $"Expected a positive vector score, but got {candidate.VectorScore.Value}.");
        }

    }
}
