namespace Baseera.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string NormalizedName { get; set; } = null!;

        public IReadOnlyList<double> Embedding { get; set; } = [];

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<Alias> Aliases { get; set; } = [];
    }
}
