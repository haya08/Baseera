using Baseera.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baseera.Infrastructure.Configurations
{
    public class TbSearchTermConfigurations : IEntityTypeConfiguration<TbSearchTerm>
    {
        public void Configure(EntityTypeBuilder<TbSearchTerm> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(x => x.Type)
                .HasConversion<int>();

            builder.Property(x => x.Priority)
                .HasDefaultValue(1);

            builder.Property(x => x.Language)
                .HasMaxLength(10);

            builder.HasOne(x => x.SearchKnowledgeProfile)
                .WithMany(x => x.SearchTerms)
                .HasForeignKey(x => x.SearchKnowledgeId);

            builder.HasIndex(x => new
            {
                x.SearchKnowledgeId,
                x.Type
            });

            builder.HasIndex(x => x.Value);

            builder.HasIndex(x => new
            {
                x.SearchKnowledgeId,
                x.Type,
                x.Value
            }).IsUnique();
        }
    }
}
