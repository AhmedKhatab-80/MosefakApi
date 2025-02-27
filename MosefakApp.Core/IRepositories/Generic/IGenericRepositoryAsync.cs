namespace MosefakApp.Core.IRepositories.Generic
{
    public interface IGenericRepositoryAsync<T> where T : class
    {
        Task<IList<T>> GetAllAsync();
        Task<IList<T>> GetAllAsync(IEnumerable<string> includes = null!);
        Task<IList<T>> GetAllAsync(Expression<Func<T,bool>> expression, IEnumerable<string> includes = null!);
        Task<T> GetByIdAsync(object id);
        Task<double> GetAverage(Expression<Func<T,double>> expression, Expression<Func<T,bool>> criteria);
        Task<double> GetAverage(Expression<Func<T,double>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null);
        Task<T> FirstOrDefaultASync(Expression<Func<T, bool>> predicate, string[] includes = null!);
        Task<long> GetCountAsync();
        Task<long> GetCountWithConditionAsync(Expression<Func<T, bool>> condition);
        Task<long> GetCountWithConditionAsync(Expression<Func<T, bool>> condition, string[] includes = null!);
        Task AddEntityAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateEntityAsync(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entities);
        Task DeleteEntityAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);
    }
}
