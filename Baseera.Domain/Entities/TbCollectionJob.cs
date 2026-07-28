using Baseera.Domain.Enums;

namespace Baseera.Domain.Entities
{
    public class TbCollectionJob
    {
        #region Fields
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public JobStatus Status { get; set; }
        #endregion


        #region Relationships
        public TbBrand Brand { get; set; }
        public ICollection<TbQueryExecution> QueryExecutions { get; set; }
        #endregion
    }
}
