namespace Baseera.Infrastructure.KnowledgeResolution.Gemini.Models
{
    internal sealed class GeminiResolutionDecision
    {
        public string Decision { get; init; } = null!;

        public string? CandidateEntityId { get; init; }

        public double Confidence { get; init; }

        public string? Reason { get; init; }
    }
}
