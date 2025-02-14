namespace MosefakApp.Core.Dtos.Review.Requests
{
    public class ReviewRequest
    {
        [Required]
        [Range(1, 5)]
        public int Rate { get; set; } // validate if in range 1:5 stars in Service Layer and in Dto like that [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public string? Comment { get; set; }
    }
}
