namespace MosefakApp.Core.IRepositories.Non_Generic
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllUsersAsync(int pageIndex = 1, int pageSize = 10);
        Task<IEnumerable<AppUser>> GetAllUsersAsync(Expression<Func<AppUser, bool>> expression, int pageIndex = 1, int pageSize = 10);
        Task<AppUser?> GetUserByIdAsync(int id);
        Task UpdateUser(AppUser user);
        Task<int> Save();
    }
}
