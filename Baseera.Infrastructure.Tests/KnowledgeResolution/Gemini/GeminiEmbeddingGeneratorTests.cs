using Baseera.Infrastructure.KnowledgeResolution.Gemini;
using Baseera.Infrastructure.Tests.TestSetups;
using Google.GenAI;
using Xunit;

namespace Baseera.Infrastructure.Tests.KnowledgeResolution.Gemini
{
    public class GeminiEmbeddingGeneratorTests
    {
        [Fact]
        public async Task GenerateAsync_Returns768DimensionalEmbedding()
        {
            TestEnvironment.Load();

            // Arrange
            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");

            Assert.False(
                string.IsNullOrWhiteSpace(apiKey),
                "GEMINI_API_KEY environment variable is missing.");

            var client = new Client(
                apiKey: apiKey);

            var generator = new GeminiEmbeddingGenerator(client);

            const string text =
                "Nike Pegasus is a running shoe designed for runners.";

            // Act
            var embedding = await generator.GenerateAsync(text);

            // Assert
            Assert.NotNull(embedding);
            Assert.NotEmpty(embedding);

            Assert.Equal(768, embedding.Count);
        }
    }
}
