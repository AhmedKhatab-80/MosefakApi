namespace MosefakApp.Core.Dtos.Review.Responses
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public int Rate { get; set; } 
        public string? Comment { get; set; }
        public int AppUserId { get; set; } 
    }
}
