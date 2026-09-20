using Baseera.Core.KnowledgeResolution.Abstracts;

namespace Baseera.Infrastructure.Tests.KnowledgeResolution.Fakes
{
    public sealed class FakeEmbeddingGenerator : IEmbeddingGenerator
    {
        public Task<IReadOnlyList<double>> GenerateAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            var embedding = new double[768];

            // Deterministic test vector
            embedding[0] = 1.0;

            return Task.FromResult<IReadOnlyList<double>>(embedding);
        }
    }
}
