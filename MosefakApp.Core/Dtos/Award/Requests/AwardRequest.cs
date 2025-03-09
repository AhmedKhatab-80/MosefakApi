namespace MosefakApp.Core.Dtos.Award.Requests
{
    public class AwardRequest
    {
        public string Title { get; set; } = null!; // "Best Cardiologist 2024"
        public DateOnly DateReceived { get; set; } // When the award was received
        public string Organization { get; set; } = null!;
        public string? Description { get; set; }
    }
}
