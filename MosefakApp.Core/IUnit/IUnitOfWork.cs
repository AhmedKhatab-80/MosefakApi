namespace MosefakApp.Core.IUnit
{
    public interface IUnitOfWork : IAsyncDisposable 
    {
        IGenericRepositoryAsync<T> Repository<T>() where T : class;
        TRepository GetCustomRepository<TRepository>() where TRepository : class;
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
