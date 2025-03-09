namespace MosefakApp.Core.Dtos.Specialization.Responses
{
    public class SpecializationResponse
    {
        public string Id { get; set; } = null!;
        public Specialty Name { get; set; } // Cardiology طب القلب
        public SpecialtyCategory Category { get; set; } // Heart القلب
    }
}
