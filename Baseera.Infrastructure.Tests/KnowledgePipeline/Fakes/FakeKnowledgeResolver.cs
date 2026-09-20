using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Abstracts;
using Baseera.Core.KnowledgeResolution.Models;

namespace Baseera.Infrastructure.Tests.KnowledgePipeline.Fakes;

internal sealed class FakeKnowledgeResolver : IKnowledgeResolver
{
    public bool WasCalled { get; private set; }

    public KnowledgeExtractionResult? ReceivedExtractionResult { get; private set; }

    public ResolvedKnowledge Result { get; set; } = new();

    public Exception? ExceptionToThrow { get; set; }

    public Task<ResolvedKnowledge> ResolveAsync(
        KnowledgeExtractionResult extractionResult,
        CancellationToken cancellationToken = default)
    {
        WasCalled = true;
        ReceivedExtractionResult = extractionResult;

        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        return Task.FromResult(Result);
    }
}
