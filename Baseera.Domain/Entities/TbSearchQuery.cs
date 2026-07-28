using Baseera.Domain.Enums;

namespace Baseera.Domain.Entities
{
    public class TbSearchQuery
    {
        #region Fields
        public Guid Id { get; set; }
        public Guid SearchKnowledgeId { get; set; }
        public Platform Platform { get; set; }
        public QueryIntent Intent { get; set; }
        public string QueryText { get; set; }
        public string Status { get; set; }
        public int Version { get; set; }
        public DateTime LastExecutedAt { get; set; }
        public DateTime NextExecutionAt { get; set; }
        public DateTime CreatedAt { get; set; }
        #endregion


        #region Relationships
        public TbSearchKnowledgeProfile SearchKnowledgeProfile { get; set; }
        public ICollection<TbQueryExecution> QueryExecutions { get; set; }
        #endregion
    }
}
