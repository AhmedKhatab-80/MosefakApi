namespace MosefakApp.Shared.Interfaces
{
    public interface IEntityModificationHistory
    {
        DateTime? FirstUpdatedTime { get; set; }
        DateTime? LastUpdatedTime { get; set; }
        int? FirstUpdatedByUserId { get; set; }
        int? LastUpdatedByUserId { get; set; }
    }

}
