namespace MosefakApp.Core.IUnit
{
    public interface IUnitOfWork : IAsyncDisposable 
    {
        IGenericRepositoryAsync<T> Repository<T>() where T : class;
        TRepository GetCustomRepository<TRepository>() where TRepository : class;
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync();
        IExecutionStrategy CreateExecutionStrategy(); 
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
