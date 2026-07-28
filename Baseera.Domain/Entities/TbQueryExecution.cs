namespace Baseera.Domain.Entities
{
    public class TbQueryExecution
    {
        #region Fields
        public Guid Id { get; set; }
        public Guid QueryId { get; set; }
        public Guid CollectionJobId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public string Status { get; set; }
        public int DocumentsCollected { get; set; }
        public string ErrorMessage { get; set; }
        #endregion


        #region Relationships
        public TbCollectionJob CollectionJob { get; set; }
        public TbSearchQuery SearchQuery { get; set; }
        #endregion

    }
}
