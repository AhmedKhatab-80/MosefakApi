namespace MosefakApp.Shared.Interfaces
{
    public interface ISoftDeletable
    {
        bool IsSoftDeleted { get; set; }
        DateTime? DeletedTime { get; set; }
        int? DeletedByUserId { get; set; }

        Task SoftDeleteAsync()
        {
            IsSoftDeleted = true;
            DeletedTime = DateTime.Now;

            return Task.CompletedTask;
        }

        Task UndoDelete()
        {
            IsSoftDeleted = false;
            DeletedTime = null;

            return Task.CompletedTask;
        }
    }

}
