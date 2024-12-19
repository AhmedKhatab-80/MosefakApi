namespace MosefakApp.Domains.Entities.Identity
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Address Address { get; set; } = null!;
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public bool IsDisabled { get; set; } = false;
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
