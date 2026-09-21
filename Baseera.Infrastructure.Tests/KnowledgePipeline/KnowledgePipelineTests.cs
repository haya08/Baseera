using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Models;
using Baseera.Domain.Enums;
using Baseera.Infrastructure.Tests.KnowledgePipeline.Fakes;
using Baseera.Service.KnowledgePipeline;
using Xunit;

namespace Baseera.Infrastructure.Tests.KnowledgePipeline;

public class KnowledgePipelineTests
{
    [Fact]
    public async Task ProcessAsync_ExtractsResolvesAndPersists()
    {
        // Arrange
        var extractor = new FakeKnowledgeExtractor();
        var resolver = new FakeKnowledgeResolver();
        var persistence = new FakeKnowledgePersistenceService();

        var document = CreateDocument();

        var extractionResult = new KnowledgeExtractionResult
        {
            Entities = [],
            Relationships = []
        };

        var resolvedKnowledge = new ResolvedKnowledge
        {
            Entities = [],
            Relationships = []
        };

        extractor.Result = extractionResult;
        resolver.Result = resolvedKnowledge;

        var pipeline = new KnowledgePipelineService(
            extractor,
            resolver,
            persistence);

        // Act
        var result = await pipeline.ProcessAsync(document);

        // Assert
        Assert.True(extractor.WasCalled);
        Assert.True(resolver.WasCalled);
        Assert.True(persistence.WasCalled);

        Assert.Same(
            document,
            extractor.ReceivedDocument);

        Assert.Same(
            extractionResult,
            resolver.ReceivedExtractionResult);

        Assert.Same(
            extractionResult,
            persistence.ReceivedExtractionResult);

        Assert.Same(
            resolvedKnowledge,
            persistence.ReceivedResolvedKnowledge);

        Assert.Same(
            resolvedKnowledge,
            result);
    }

    [Fact]
    public async Task ProcessAsync_ExtractionFails_DoesNotResolveOrPersist()
    {
        // Arrange
        var extractor = new FakeKnowledgeExtractor
        {
            ExceptionToThrow = new InvalidOperationException(
                "Extraction failed.")
        };

        var resolver = new FakeKnowledgeResolver();
        var persistence = new FakeKnowledgePersistenceService();

        var document = CreateDocument();

        var pipeline = new KnowledgePipelineService(
            extractor,
            resolver,
            persistence);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => pipeline.ProcessAsync(document));

        // Assert
        Assert.Equal(
            "Extraction failed.",
            exception.Message);

        Assert.True(extractor.WasCalled);

        Assert.Same(
            document,
            extractor.ReceivedDocument);

        Assert.False(resolver.WasCalled);

        Assert.False(persistence.WasCalled);
    }

    [Fact]
    public async Task ProcessAsync_ResolutionFails_DoesNotPersist()
    {
        // Arrange
        var extractor = new FakeKnowledgeExtractor();

        var resolver = new FakeKnowledgeResolver
        {
            ExceptionToThrow = new InvalidOperationException(
                "Resolution failed.")
        };

        var persistence = new FakeKnowledgePersistenceService();

        var document = CreateDocument();

        var pipeline = new KnowledgePipelineService(
            extractor,
            resolver,
            persistence);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => pipeline.ProcessAsync(document));

        // Assert
        Assert.Equal(
            "Resolution failed.",
            exception.Message);

        Assert.True(extractor.WasCalled);

        Assert.Same(
            document,
            extractor.ReceivedDocument);

        Assert.True(resolver.WasCalled);

        Assert.False(persistence.WasCalled);
    }

    private static UnifiedRawDocument CreateDocument()
    {
        return new UnifiedRawDocument
        {
            ExternalId = "post_123",
            Platform = Platform.Instagram,
            Author = "Nike",
            Title = "New Pegasus",
            Body = "The new Pegasus is lighter and more comfortable.",
            PublishedAt = DateTimeOffset.UtcNow,
            CollectedAt = DateTimeOffset.UtcNow,
            Comments =
            [
                new UnifiedComment
            {
                ExternalId = "comment_1",
                Author = "Ahmed",
                Body = "I bought these for running.",
                PublishedAt = DateTimeOffset.UtcNow,
                Score = 10,
                Depth = 0
            }
            ],
            RawJson = "{}"
        };
    }
}
