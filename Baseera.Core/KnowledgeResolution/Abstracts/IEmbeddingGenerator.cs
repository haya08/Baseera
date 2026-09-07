namespace Baseera.Core.KnowledgeResolution.Abstracts
{
    public interface IEmbeddingGenerator
    {
        Task<IReadOnlyList<double>> GenerateAsync(
            string text,
            CancellationToken cancellationToken = default);
    }
}
