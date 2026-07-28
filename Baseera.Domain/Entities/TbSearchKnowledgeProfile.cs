using Baseera.Domain.Enums;

namespace Baseera.Domain.Entities
{
    public class TbSearchKnowledgeProfile
    {
        #region Fields
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public int Version { get; set; }
        public SearchKnowledgeStatus Status { get; set; }
        public string GeneratedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        #endregion


        #region Relationships
        public TbBrand Brand { get; set; }
        public ICollection<TbSearchTerm> SearchTerms { get; set; }
        public ICollection<TbSearchQuery> SearchQueries { get; set; }
        #endregion
    }
}
