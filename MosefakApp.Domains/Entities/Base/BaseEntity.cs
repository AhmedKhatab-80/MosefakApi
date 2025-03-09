namespace MosefakApp.Domains.Entities.Base
{
    public abstract class BaseEntity : IAuditableEntity, ISoftDeletable
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public int CreatedByUserId { get; set; }
        public DateTimeOffset? FirstUpdatedTime { get; set; }
        public DateTimeOffset? LastUpdatedTime { get; set; }
        public int? FirstUpdatedByUserId { get; set; }
        public int? LastUpdatedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset? DeletedTime { get; set; }
        public int? DeletedByUserId { get; set; }
        public void MarkAsDeleted(int userId)
        {
            IsDeleted = true;
            DeletedByUserId = userId;
            DeletedTime = DateTimeOffset.UtcNow;
        }
    }
}
