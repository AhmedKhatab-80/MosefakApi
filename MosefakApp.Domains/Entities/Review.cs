namespace MosefakApp.Domains.Entities
{
    public class Review : BaseEntity
    {
        public int Rate { get; set; } // validate if in range 1:5 stars in Service Layer and in Dto like that [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public string? Comment { get; set; }
        public int AppUserId { get; set; } // fk for AppUser to represent Patient and didn't put navigation because it's exist in another DB
    }
}
