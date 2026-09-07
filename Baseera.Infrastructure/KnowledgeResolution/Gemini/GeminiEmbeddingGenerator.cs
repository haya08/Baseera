using Baseera.Core.KnowledgeResolution.Abstracts;
using Google.GenAI;
using Google.GenAI.Types;

namespace Baseera.Infrastructure.KnowledgeResolution.Gemini
{
    public class GeminiEmbeddingGenerator : IEmbeddingGenerator
    {
        private readonly Client _client;

        public GeminiEmbeddingGenerator(Client client)
        {
            _client = client;
        }

        public async Task<IReadOnlyList<double>> GenerateAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            var response = await _client.Models.EmbedContentAsync(
                model: "gemini-embedding-2",
                contents: text,
                config: new EmbedContentConfig
                {
                    OutputDimensionality = 768
                },
                cancellationToken: cancellationToken);

            return response.Embeddings[0].Values;
        }
    }
}
