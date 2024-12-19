namespace MosefakApp.Core.IUnit
{
    public interface IUnitOfWork<T> : IAsyncDisposable where T : BaseEntity
    {
        IGenericRepositoryAsync<T> RepositoryAsync { get; }
        Task<int> SaveAsync();
        Task CommitAsync();
        Task RollBackAsync();

    }
}
