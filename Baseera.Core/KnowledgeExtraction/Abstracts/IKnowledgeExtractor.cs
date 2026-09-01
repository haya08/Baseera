using Baseera.Core.Documents.Models;

namespace Baseera.Core.KnowledgeExtraction.Abstracts
{
    public interface IKnowledgeExtractor
    {
        Task<KnowledgeExtractionResult> ExtractAsync(
            string text,
            CancellationToken cancellationToken = default);
    }
}
