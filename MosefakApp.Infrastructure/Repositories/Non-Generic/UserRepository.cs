namespace MosefakApp.Infrastructure.Repositories.Non_Generic
{
    public class UserRepository : IUserRepository
    {
        private readonly AppIdentityDbContext _context;

        public UserRepository(AppIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<AppUser?> GetUserByIdAsync(int id) => await _context.Users.FindAsync(id);

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync() => await _context.Users.ToListAsync();

        public async Task UpdateUser(AppUser user)
        {
            await Task.Run(() =>
            {
                _context.Users.Update(user);
            });
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
