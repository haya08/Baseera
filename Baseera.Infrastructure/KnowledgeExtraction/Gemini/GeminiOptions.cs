namespace Baseera.Infrastructure.KnowledgeExtraction.Gemini
{
    public sealed class GeminiOptions
    {
        public string ApiKey { get; set; } = null!;

        public string Model { get; set; } = "gemini-3.6-flash";
    }
}
