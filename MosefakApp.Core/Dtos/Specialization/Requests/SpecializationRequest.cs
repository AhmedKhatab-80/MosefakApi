namespace MosefakApp.Core.Dtos.Specialization.Requests
{
    public class SpecializationRequest
    {
        [Required]
        public Specialty Name { get; set; } // Cardiology طب القلب

        [Required]
        public SpecialtyCategory Category { get; set; } // Heart القلب
    }
}
