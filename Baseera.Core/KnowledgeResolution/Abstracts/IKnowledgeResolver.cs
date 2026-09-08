using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Models;

namespace Baseera.Core.KnowledgeResolution.Abstracts
{
    public interface IKnowledgeResolver
    {
        Task<ResolvedKnowledge> ResolveAsync(
            KnowledgeExtractionResult extractionResult,
            CancellationToken cancellationToken = default);
    }
}
