namespace MosefakApp.Shared.Interfaces
{
    public interface IAuditableEntity : IEntityCreationTime, IEntityCreatedByUser, IEntityModificationHistory
    {

    }
}
