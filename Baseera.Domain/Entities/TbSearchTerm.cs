using Baseera.Domain.Enums;

namespace Baseera.Domain.Entities
{
    public class TbSearchTerm
    {
        #region Fields
        public Guid Id { get; set; }
        public Guid SearchKnowledgeId { get; set; }
        public string Value { get; set; }
        public SearchTermType Type { get; set; }
        public int Priority { get; set; }
        public string Language { get; set; }
        #endregion


        #region Relationships
        public TbSearchKnowledgeProfile SearchKnowledgeProfile { get; set; }

        #endregion
    }
}
