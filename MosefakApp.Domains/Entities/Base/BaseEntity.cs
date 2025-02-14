namespace MosefakApp.Domains.Entities.Base
{
    public abstract class BaseEntity : IAuditableEntity, ISoftDeletable
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedByUserId { get; set; }
        public DateTime? FirstUpdatedTime { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
        public int? FirstUpdatedByUserId { get; set; }
        public int? LastUpdatedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedTime { get; set; }
        public int? DeletedByUserId { get; set; }
        public void MarkAsDeleted(int userId)
        {
            IsDeleted = true;
            DeletedByUserId = userId;
            DeletedTime = DateTime.UtcNow;
        }
    }
}
