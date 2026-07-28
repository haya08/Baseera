using Baseera.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Baseera.Infrastructure.AppContext
{
    public class BaseeraDBContext : DbContext
    {
        public BaseeraDBContext() { }

        public BaseeraDBContext(DbContextOptions<BaseeraDBContext> options)
            : base(options)
        {

        }

        public virtual DbSet<TbBrand> Brands { get; set; }
        public virtual DbSet<TbCollectionJob> CollectionJobs { get; set; }
        public virtual DbSet<TbQueryExecution> QueryExecutions { get; set; }
        public virtual DbSet<TbSearchKnowledgeProfile> SearchKnowledgeProfiles { get; set; }
        public virtual DbSet<TbSearchQuery> SearchQueries { get; set; }
        public virtual DbSet<TbSearchTerm> SearchTerms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseeraDBContext).Assembly);
        }
    }
}
