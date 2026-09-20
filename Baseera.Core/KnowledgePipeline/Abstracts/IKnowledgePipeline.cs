using Baseera.Core.KnowledgeResolution.Models;

namespace Baseera.Core.KnowledgePipeline.Abstracts
{
    public interface IKnowledgePipeline
    {
        Task<ResolvedKnowledge> ProcessAsync(
            string text,
            CancellationToken cancellationToken = default);
    }
}
