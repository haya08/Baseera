namespace Baseera.Domain.Entities
{
    public class Alias
    {
        public Guid Id { get; set; }

        public Guid EntityId { get; set; }

        public string Value { get; set; } = null!;

        public string NormalizedValue { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
