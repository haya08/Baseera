using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Models;

namespace Baseera.Core.KnowledgeIngestion;

public interface IKnowledgeIngestionService
{
    Task<IReadOnlyList<ResolvedKnowledge>> ProcessAsync(
        IReadOnlyList<UnifiedRawDocument> documents,
        CancellationToken cancellationToken = default);
}
