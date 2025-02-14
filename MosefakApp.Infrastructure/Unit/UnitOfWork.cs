namespace MosefakApp.Infrastructure.Unit
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();
        private readonly Dictionary<Type, object> _customRepositories = new();
        private IDbContextTransaction _transaction;


        public UnitOfWork(AppDbContext context, IEnumerable<object> customRepositories)
        {
            _context = context;

            // Add custom repositories
            foreach (var repo in customRepositories)
            {
                _customRepositories[repo.GetType()] = repo;
            }
        }

        // ✅ Generic repository for standard CRUD
        public IGenericRepositoryAsync<T> Repository<T>() where T : class
        {
            if (!_repositories.TryGetValue(typeof(T), out var repo))
            {
                repo = new GenericRepositoryAsync<T>(_context);
                _repositories[typeof(T)] = repo;
            }

            return (IGenericRepositoryAsync<T>)repo;
        }

        // ✅ Retrieve custom repositories (specialized methods)
        public TRepository? GetCustomRepository<TRepository>() where TRepository : class
        {
            return _customRepositories.TryGetValue(typeof(TRepository), out var repository)
                ? (TRepository)repository
                : throw new InvalidOperationException($"Repository of type {typeof(TRepository).Name} is not registered.");
        }


        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _transaction = await _context.Database.BeginTransactionAsync();
            return _transaction;
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress.");
            }

            try
            {
                await _transaction.CommitAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress.");
            }

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

    }
}
