using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Models;

namespace Baseera.Core.KnowledgePipeline.Abstracts
{
    public interface IKnowledgePipeline
    {
        Task<ResolvedKnowledge> ProcessAsync(
            UnifiedRawDocument document,
            CancellationToken cancellationToken = default);
    }
}
