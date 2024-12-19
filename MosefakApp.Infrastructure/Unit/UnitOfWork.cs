namespace MosefakApp.Infrastructure.Unit
{
    public class UnitOfWork<T> : IUnitOfWork<T> where T : BaseEntity
    {
        private readonly AppDbContext _appDbContext;
        public IGenericRepositoryAsync<T> RepositoryAsync { get; }

        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            RepositoryAsync = new GenericRepositoryAsync<T>(appDbContext);
        }


        public Task CommitAsync()
        {
            throw new NotImplementedException();
        }

        public async ValueTask DisposeAsync()
        {
            await _appDbContext.DisposeAsync();
        }

        public Task RollBackAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<int> SaveAsync()
        {
            return await _appDbContext.SaveChangesAsync();
        }
    }
}
