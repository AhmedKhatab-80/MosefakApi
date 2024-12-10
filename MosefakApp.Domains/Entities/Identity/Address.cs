namespace MosefakApp.Domains.Entities.Identity
{
    public class Address // that belong to user...
    {
        public int Id { get; set; }
        public string State { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public int ZipCode { get; set; }
        public int ApplicationUserId { get; set; }
        public AppUser ApplicationUser { get; set; } = null!;
    }
}
