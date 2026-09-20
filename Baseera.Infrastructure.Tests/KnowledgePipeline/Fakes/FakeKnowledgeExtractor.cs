using Baseera.Core.KnowledgeExtraction.Abstracts;
using Baseera.Core.KnowledgeExtraction.Models;

namespace Baseera.Infrastructure.Tests.KnowledgePipeline.Fakes;

internal sealed class FakeKnowledgeExtractor : IKnowledgeExtractor
{
    public bool WasCalled { get; private set; }

    public KnowledgeExtractionResult Result { get; set; } = new();

    public Exception? ExceptionToThrow { get; set; }

    public Task<KnowledgeExtractionResult> ExtractAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        WasCalled = true;

        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        return Task.FromResult(Result);
    }
}
