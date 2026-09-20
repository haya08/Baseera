using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Models;
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
        var result = await pipeline.ProcessAsync(
            "Nike Pegasus");

        // Assert
        Assert.True(extractor.WasCalled);
        Assert.True(resolver.WasCalled);
        Assert.True(persistence.WasCalled);

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

        var pipeline = new KnowledgePipelineService(
            extractor,
            resolver,
            persistence);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => pipeline.ProcessAsync("Nike Pegasus"));

        // Assert
        Assert.Equal(
            "Extraction failed.",
            exception.Message);

        Assert.True(extractor.WasCalled);

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

        var pipeline = new KnowledgePipelineService(
            extractor,
            resolver,
            persistence);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => pipeline.ProcessAsync("Nike Pegasus"));

        // Assert
        Assert.Equal(
            "Resolution failed.",
            exception.Message);

        Assert.True(extractor.WasCalled);

        Assert.True(resolver.WasCalled);

        Assert.False(persistence.WasCalled);
    }
}
