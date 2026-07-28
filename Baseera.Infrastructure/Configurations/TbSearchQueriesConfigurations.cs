using Baseera.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baseera.Infrastructure.Configurations
{
    public class TbSearchQueriesConfigurations : IEntityTypeConfiguration<TbSearchQuery>
    {
        public void Configure(EntityTypeBuilder<TbSearchQuery> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.QueryText)
                .IsRequired();

            builder.Property(x => x.Platform)
                .HasConversion<int>();

            builder.Property(x => x.Intent)
                .HasConversion<int>();

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.Property(x => x.Version);

            //builder.HasOne(x => x.Brand)
            //    .WithMany(x => x.SearchQueries)
            //    .HasForeignKey(x => x.BrandId);

            builder.HasOne(x => x.SearchKnowledgeProfile)
                .WithMany(x => x.SearchQueries)
                .HasForeignKey(x => x.SearchKnowledgeId);

            builder.HasMany(x => x.QueryExecutions)
                .WithOne(x => x.SearchQuery)
                .HasForeignKey(x => x.QueryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new
            {
                x.Platform,
                x.Status,
                x.NextExecutionAt
            });

            builder.HasIndex(x => new
            {
                x.SearchKnowledgeId,
                x.Platform,
                x.QueryText
            }).IsUnique();

            //builder.HasIndex(x => x.BrandId);

        }
    }
}
