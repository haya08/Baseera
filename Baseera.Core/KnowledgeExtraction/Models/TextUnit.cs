namespace Baseera.Core.Documents.Models
{
    public sealed class TextUnit
    {
        public Guid Id { get; init; }

        public Guid DocumentId { get; init; }

        public string Content { get; init; } = null!;
    }
}
