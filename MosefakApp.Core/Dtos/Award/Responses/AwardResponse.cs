namespace MosefakApp.Core.Dtos.Award.Responses
{
    public class AwardResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!; // "Best Cardiologist 2024"
        public DateOnly DateReceived { get; set; } // When the award was received
        public string Organization { get; set; } = null!;
        public string? Description { get; set; }
        public int DoctorId { get; set; }
    }
}
