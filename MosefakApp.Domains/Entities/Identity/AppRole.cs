namespace MosefakApp.Domains.Entities.Identity
{
    public class AppRole : IdentityRole<int>
    {
        public AppRole(string roleName) : base(roleName)
        {
        }

        public AppRole()
        {
            
        }

        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}
