namespace Baseera.Domain.Entities
{
    public class TbBrand
    {
        #region Fields
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public string Industry { get; set; }
        public int CollectionFrequency { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        #endregion


        #region Relationships
        public ICollection<TbSearchKnowledgeProfile> SearchKnowledgeProfiles { get; set; }
        public ICollection<TbCollectionJob> CollectionJobs { get; set; }
        #endregion
    }
}
