namespace MosefakApp.Domains.Entities.Base
{
    public class BaseEntity : IEntityCreationTime, IEntityCreatedByUser, IEntityModificationHistory, ISoftDeletable
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime? FirstUpdatedTime { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
        public int? FirstUpdatedByUserId { get; set; }
        public int? LastUpdatedByUserId { get; set; }
        public bool IsSoftDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public int? DeletedByUserId { get; set; }
    }
}
