namespace Baseera.Core.KnowledgeResolution.Componants
{
    public static class EmbeddingTextBuilder
    {
        public static string Build(
            string name,
            string type,
            string? description)
        {
            return $"""
            Name: {name}
            Type: {type}
            Description: {description ?? string.Empty}
            """;
        }
    }
}
