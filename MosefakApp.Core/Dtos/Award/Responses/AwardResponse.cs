namespace MosefakApp.Core.Dtos.Award.Responses
{
    public class AwardResponse
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!; // "Best Cardiologist 2024"
        public DateOnly DateReceived { get; set; } // When the award was received
        public string Organization { get; set; } = null!;
        public string? Description { get; set; }
    }
}
