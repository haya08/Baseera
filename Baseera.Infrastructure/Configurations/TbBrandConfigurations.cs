using Baseera.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baseera.Infrastructure.Configurations
{
    public class TbBrandConfigurations : IEntityTypeConfiguration<TbBrand>
    {
        public void Configure(EntityTypeBuilder<TbBrand> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Website)
                .HasMaxLength(500);

            builder.Property(x => x.Description)
                .HasMaxLength(4000);

            builder.Property(x => x.Industry)
                .HasMaxLength(100);

            builder.Property(x => x.CollectionFrequency)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            builder.HasMany(x => x.SearchKnowledgeProfiles)
                .WithOne(x => x.Brand)
                .HasForeignKey(x => x.BrandId)
                .OnDelete(DeleteBehavior.Cascade);

            //builder.HasMany(x => x.SearchQueries)
            //    .WithOne(x => x.Brand)
            //    .HasForeignKey(x => x.BrandId)
            //    .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.CollectionJobs)
                .WithOne(x => x.Brand)
                .HasForeignKey(x => x.BrandId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Name);
        }
    }
}
