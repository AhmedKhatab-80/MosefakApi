using MosefakApp.Domains.Enums;

namespace MosefakApp.Core.Dtos.Specialization.Responses
{
    public class SpecializationResponse
    {
        public int Id { get; set; }
        public Specialty Name { get; set; } // Cardiology طب القلب
        public SpecialtyCategory Category { get; set; } // Heart القلب
        public int DoctorId { get; set; }
    }
}
