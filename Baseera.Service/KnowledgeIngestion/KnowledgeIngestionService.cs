using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeIngestion;
using Baseera.Core.KnowledgePipeline.Abstracts;
using Baseera.Core.KnowledgeResolution.Models;

namespace Baseera.Service.KnowledgeIngestion;

public sealed class KnowledgeIngestionService
    : IKnowledgeIngestionService
{
    private readonly IKnowledgePipeline _pipeline;

    public KnowledgeIngestionService(
        IKnowledgePipeline pipeline)
    {
        _pipeline = pipeline;
    }

    public async Task<IReadOnlyList<ResolvedKnowledge>> ProcessAsync(
        IReadOnlyList<UnifiedRawDocument> documents,
        CancellationToken cancellationToken = default)
    {
        var results = new List<ResolvedKnowledge>();

        foreach (var document in documents)
        {
            var result = await _pipeline.ProcessAsync(
                document,
                cancellationToken);

            results.Add(result);
        }

        return results;
    }
}
