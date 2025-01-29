namespace MosefakApp.Shared.Interfaces
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; } 
        DateTime? DeletedTime { get; set; }
        int? DeletedByUserId { get; set; }
    }
}
