using Baseera.Core.Documents.Enums;
using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Enums;
using Baseera.Core.KnowledgeResolution.Models;
using Baseera.Infrastructure.KnowledgeResolution.Gemini;
using Baseera.Infrastructure.Options;
using Baseera.Infrastructure.Tests.TestSetups;
using Google.GenAI;
using Xunit;

namespace Baseera.Infrastructure.Tests.KnowledgeResolution.Gemini
{
    public class GeminiResolutionJudgeTests
    {
        [Fact]
        public async Task JudgeAsync_WhenEntityMatchesCandidate_ReturnsMatch()
        {
            TestEnvironment.Load();

            // Arrange

            var apiKey = Environment.GetEnvironmentVariable(
                "GEMINI_API_KEY")
                ?? throw new InvalidOperationException(
                    "GEMINI_API_KEY is not configured.");

            var options = Microsoft.Extensions.Options.Options.Create(new GeminiOptions
            {
                ApiKey = apiKey,
                Model = "gemini-3.1-flash-lite"
            });

            var client = new Client(
                apiKey: options.Value.ApiKey);

            var judge = new GeminiResolutionJudge(
                client,
                options);

            var nikeId = Guid.NewGuid();

            var candidates = new List<ResolutionCandidate>
            {
                new()
                {
                    EntityId = nikeId,
                    Name = "Nike",
                    Type = EntityType.Brand,
                    Description =
                        "An American multinational corporation specializing in athletic footwear, apparel, and sports equipment."
                },

                new()
                {
                    EntityId = Guid.NewGuid(),
                    Name = "Adidas",
                    Type = EntityType.Brand,
                    Description =
                        "A German multinational corporation that designs and manufactures athletic shoes, clothing, and accessories."
                }
            };

            var entity = new ExtractedEntity
            {
                Name = "Puma",
                Type = EntityType.Brand,
                Description =
                    "A global sportswear company known for athletic shoes and apparel.",
                Confidence = 0.98
            };

            // Act

            var result = await judge.JudgeAsync(
                entity,
                candidates);

            // Assert

            Assert.Equal(
                ResolutionDecisionType.Match, // expected
                result.Decision);

            Assert.Equal(
                null,
                result.CandidateEntityId);

            Assert.InRange(
                result.Confidence,
                0,
                1);
        }

        [Fact]
        public async Task JudgeAsync_WhenMultipleCandidatesArePlausible_ReturnsAmbiguous()
        {
            TestEnvironment.Load();

            // Arrange
            var apiKey = Environment.GetEnvironmentVariable(
                "GEMINI_API_KEY")
                ?? throw new InvalidOperationException(
                    "GEMINI_API_KEY is not configured.");

            var options = Microsoft.Extensions.Options.Options.Create(new GeminiOptions
            {
                ApiKey = apiKey,
                Model = "gemini-3.6-flash"
            });

            var client = new Client(apiKey: apiKey);

            var judge = new GeminiResolutionJudge(
                client,
                options);

            var candidates = new List<ResolutionCandidate>
            {
                new()
                {
                    EntityId = Guid.NewGuid(),
                    Name = "Michael Johnson",
                    Type = EntityType.Person,
                    Description =
                        "A marketing manager working in the sports industry."
                },
                new()
                {
                    EntityId = Guid.NewGuid(),
                    Name = "Michael Johnson",
                    Type = EntityType.Person,
                    Description =
                        "A software engineer working in the technology industry."
                }
            };

            var entity = new ExtractedEntity
            {
                Id = "e3",
                Name = "Michael Johnson",
                Type = EntityType.Person,
                Description =
                    "Michael Johnson.",
                Confidence = 0.95
            };

            // Act
            var result = await judge.JudgeAsync(
                entity,
                candidates);

            // Assert
            Assert.Equal(
                ResolutionDecisionType.Ambiguous,
                result.Decision);

            Assert.Null(
                result.CandidateEntityId);

            Assert.InRange(
                result.Confidence,
                0,
                1);
        }
    }
}
