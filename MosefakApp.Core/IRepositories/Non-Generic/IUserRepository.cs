namespace MosefakApp.Core.IRepositories.Non_Generic
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<IEnumerable<AppUser>> GetAllUsersAsync(Expression<Func<AppUser, bool>> expression);
        Task<AppUser?> GetUserByIdAsync(int id);
        Task UpdateUser(AppUser user);
        Task<int> Save();
    }
}
