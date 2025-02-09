namespace MosefakApp.Core.IRepositories.Non_Generic
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<AppUser?> GetUserByIdAsync(int id);
        Task UpdateUser(AppUser user);
        Task<int> Save();
    }
}
