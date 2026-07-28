using Baseera.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baseera.Infrastructure.Configurations
{
    public class TbSearchKnowledgeConfigurations : IEntityTypeConfiguration<TbSearchKnowledgeProfile>
    {
        public void Configure(EntityTypeBuilder<TbSearchKnowledgeProfile> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Version)
                .IsRequired();

            builder.Property(x => x.GeneratedBy)
                .HasMaxLength(100);

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.Brand)
                .WithMany(x => x.SearchKnowledgeProfiles)
                .HasForeignKey(x => x.BrandId);

            builder.HasMany(x => x.SearchTerms)
                .WithOne(x => x.SearchKnowledgeProfile)
                .HasForeignKey(x => x.SearchKnowledgeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.SearchQueries)
                .WithOne(x => x.SearchKnowledgeProfile)
                .HasForeignKey(x => x.SearchKnowledgeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.BrandId,
                x.Version
            }).IsUnique();
        }
    }
}
