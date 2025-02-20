namespace MosefakApp.Shared.Interfaces
{
    public interface IEntityModificationHistory
    {
        DateTimeOffset? FirstUpdatedTime { get; set; }
        DateTimeOffset? LastUpdatedTime { get; set; }
        int? FirstUpdatedByUserId { get; set; }
        int? LastUpdatedByUserId { get; set; }
    }

}
