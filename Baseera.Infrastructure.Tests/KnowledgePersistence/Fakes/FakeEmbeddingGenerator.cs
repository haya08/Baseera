using Baseera.Core.KnowledgeResolution.Abstracts;

namespace Baseera.Infrastructure.Tests.KnowledgePersistence.Fakes;

internal sealed class FakeEmbeddingGenerator : IEmbeddingGenerator
{
    public List<string> GeneratedTexts { get; } = [];

    public Task<IReadOnlyList<double>> GenerateAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        GeneratedTexts.Add(text);

        return Task.FromResult<IReadOnlyList<double>>(
            Enumerable.Repeat(0.1, 768).ToArray());
    }
}
