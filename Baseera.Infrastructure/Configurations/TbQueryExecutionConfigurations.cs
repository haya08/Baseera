using Baseera.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baseera.Infrastructure.Configurations
{
    public class TbQueryExecutionConfigurations : IEntityTypeConfiguration<TbQueryExecution>
    {
        public void Configure(EntityTypeBuilder<TbQueryExecution> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(4000);

            builder.HasOne(x => x.SearchQuery)
                .WithMany(x => x.QueryExecutions)
                .HasForeignKey(x => x.QueryId);

            builder.HasOne(x => x.CollectionJob)
                .WithMany(x => x.QueryExecutions)
                .HasForeignKey(x => x.CollectionJobId);

            builder.HasIndex(x => x.QueryId);

            builder.HasIndex(x => x.StartedAt);
        }
    }
}
