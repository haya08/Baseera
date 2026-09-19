using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Models;

namespace Baseera.Core.KnowledgePersistence.Abstracts;

public interface IKnowledgePersistenceService
{
    Task PersistAsync(
        KnowledgeExtractionResult extractedKnowledge,
        ResolvedKnowledge resolvedKnowledge,
        CancellationToken cancellationToken = default);
}
