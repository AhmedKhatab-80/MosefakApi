namespace MosefakApp.Infrastructure.Repositories.Generic
{
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;
        private DbSet<T> _entity; 

        public GenericRepositoryAsync(AppDbContext context)
        {
            _context = context;
            _entity = _context.Set<T>();
        }

        public DbSet<T> entity { get { return _entity; } }
    }
}
