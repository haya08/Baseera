using Baseera.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baseera.Infrastructure.Configurations
{
    public class TbCollectionJobConfigurations : IEntityTypeConfiguration<TbCollectionJob>
    {
        public void Configure(EntityTypeBuilder<TbCollectionJob> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.HasOne(x => x.Brand)
                .WithMany(x => x.CollectionJobs)
                .HasForeignKey(x => x.BrandId);

            builder.HasMany(x => x.QueryExecutions)
                .WithOne(x => x.CollectionJob)
                .HasForeignKey(x => x.CollectionJobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new
            {
                x.BrandId,
                x.StartedAt
            });
        }
    }
}
