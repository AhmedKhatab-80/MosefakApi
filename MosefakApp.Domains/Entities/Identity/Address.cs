namespace MosefakApp.Domains.Entities.Identity
{
    public class Address // that belong to user...
    {
        public int Id { get; set; }
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; } = null!;
    }
}
