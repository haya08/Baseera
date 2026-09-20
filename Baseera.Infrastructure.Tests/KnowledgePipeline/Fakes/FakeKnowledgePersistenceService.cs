using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgePersistence.Abstracts;
using Baseera.Core.KnowledgeResolution.Models;

namespace Baseera.Infrastructure.Tests.KnowledgePipeline.Fakes;

internal sealed class FakeKnowledgePersistenceService
    : IKnowledgePersistenceService
{
    public bool WasCalled { get; private set; }

    public KnowledgeExtractionResult? ReceivedExtractionResult { get; private set; }

    public ResolvedKnowledge? ReceivedResolvedKnowledge { get; private set; }

    public Task PersistAsync(
        KnowledgeExtractionResult extractedKnowledge,
        ResolvedKnowledge resolvedKnowledge,
        CancellationToken cancellationToken = default)
    {
        WasCalled = true;

        ReceivedExtractionResult = extractedKnowledge;
        ReceivedResolvedKnowledge = resolvedKnowledge;

        return Task.CompletedTask;
    }
}