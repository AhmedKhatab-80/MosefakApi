namespace MosefakApp.Infrastructure.Repositories.Generic
{
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : class
    {
        private readonly AppDbContext _context;
        private DbSet<T> _entity; 

        public GenericRepositoryAsync(AppDbContext context)
        {
            _context = context;
            _entity = _context.Set<T>();
        }

        public DbSet<T> Entity { get { return _entity; } }

        public async Task AddEntityAsync(T entity)
        {
            await _entity.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _entity.AddRangeAsync(entities);
        }

        public async Task DeleteEntityAsync(T entity)
        {
            await Task.Run(() =>
            {
                _entity.Remove(entity);
            });
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            await Task.Run(() =>
            {
                _entity.RemoveRange(entities);
            });
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null
            ? await _entity.AnyAsync()
                : await _entity.AnyAsync(predicate);
        }
        public async Task<T> FirstOrDefaultASync(Expression<Func<T, bool>> predicate, string[] includes = null!)
        {
            IQueryable<T> query = _entity.AsQueryable();

            if(includes != null)
            {
                foreach(var include in includes)
                    query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate) ?? null!;
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await _entity.ToListAsync();
        }

        public async Task<IList<T>> GetAllAsync(IEnumerable<string> includes = null!)
        {
            IQueryable<T> query = _entity.AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> expression, IEnumerable<string> includes = null!)
        {
            IQueryable<T> query = _entity.Where(expression).AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await _entity.FindAsync(id) ?? null!;
        }

        public async Task<long> GetCountAsync()
        {
            return await _entity.CountAsync();
        }

        public async Task<long> GetCountWithConditionAsync(Expression<Func<T, bool>> condition)
        {
            return await _entity.LongCountAsync(condition);
        }

        public async Task<long> GetCountWithConditionAsync(Expression<Func<T, bool>> condition, string[] includes = null!)
        {
            IQueryable<T> query = _entity.Where(condition).AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            return await query.LongCountAsync();
        }

        public async Task UpdateEntityAsync(T entity)
        {
            await Task.Run(() =>
            {
                _entity.Update(entity);
            });
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            await Task.Run(() =>
            {
                _entity.UpdateRange(entities);
            });
        }

        public async Task<double> GetAverage(Expression<Func<T, double>> expression, Expression<Func<T, bool>> criteria)
        {
            var query = _entity.Where(criteria);

            return await query.AnyAsync() ? await query.AverageAsync(expression) : 0.0;
        }


        public async Task<double> GetAverage(Expression<Func<T, double>> expression)
        {
            return await _entity.AverageAsync(expression);
        }

    }
}
