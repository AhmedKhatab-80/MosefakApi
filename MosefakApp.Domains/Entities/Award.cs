namespace MosefakApp.Domains.Entities
{
    public class Award : BaseEntity
    {
        public string Title { get; set; } = null!; // "Best Cardiologist 2024"
        public DateOnly DateReceived { get; set; } // When the award was received
        public string Organization { get; set; } = null!;
        public string? Description { get; set; }

        // Foreign Key
        public int DoctorId { get; set; }

        // Navigation Property
        public Doctor Doctor { get; set; } = null!;
    }

}
