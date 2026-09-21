using Baseera.Core.KnowledgeExtraction.Models;

namespace Baseera.Core.KnowledgeExtraction.Abstracts
{
    public interface IKnowledgeExtractor
    {
        Task<KnowledgeExtractionResult> ExtractAsync(
            UnifiedRawDocument document,
            CancellationToken cancellationToken = default);
    }
}
