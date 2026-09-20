using Baseera.Core.KnowledgeExtraction.Abstracts;
using Baseera.Core.KnowledgePersistence.Abstracts;
using Baseera.Core.KnowledgePipeline.Abstracts;
using Baseera.Core.KnowledgeResolution.Abstracts;
using Baseera.Core.KnowledgeResolution.Models;

namespace Baseera.Service.KnowledgePipeline
{
    public sealed class KnowledgePipelineService : IKnowledgePipeline
    {
        private readonly IKnowledgeExtractor _extractor;
        private readonly IKnowledgeResolver _resolver;
        private readonly IKnowledgePersistenceService _persistenceService;

        public KnowledgePipelineService(
            IKnowledgeExtractor extractor,
            IKnowledgeResolver resolver,
            IKnowledgePersistenceService persistenceService)
        {
            _extractor = extractor;
            _resolver = resolver;
            _persistenceService = persistenceService;
        }

        public async Task<ResolvedKnowledge> ProcessAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(
                    "Text cannot be empty.",
                    nameof(text));
            }

            var extractionResult =
                await _extractor.ExtractAsync(
                    text,
                    cancellationToken);

            var resolvedKnowledge =
                await _resolver.ResolveAsync(
                    extractionResult,
                    cancellationToken);

            await _persistenceService.PersistAsync(
                extractionResult,
                resolvedKnowledge,
                cancellationToken);

            return resolvedKnowledge;
        }
    }
}
