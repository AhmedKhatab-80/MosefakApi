namespace MosefakApp.Infrastructure.Repositories.Non_Generic
{
    public class UserRepository : IUserRepository
    {
        private readonly AppIdentityDbContext _context;

        public UserRepository(AppIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync(int pageIndex = 1, int pageSize = 10)
        {
            // Ensure pageIndex is at least 1 to avoid negative skips
            pageIndex = pageIndex < 1 ? 1 : pageIndex;

            return await _context.Users.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        public async Task<AppUser?> GetUserByIdAsync(int id) => await _context.Users.FindAsync(id);


        public async Task UpdateUser(AppUser user)
        {
            await Task.Run(() =>
            {
                _context.Users.Update(user);
            });
        }

        public async Task<int> Save()
        {
           return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync(Expression<Func<AppUser, bool>> expression, int pageIndex = 1, int pageSize = 10)
        {
            // Ensure pageIndex is at least 1 to avoid negative skips
            pageIndex = pageIndex < 1 ? 1 : pageIndex;

            return await _context.Users.Where(expression).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
