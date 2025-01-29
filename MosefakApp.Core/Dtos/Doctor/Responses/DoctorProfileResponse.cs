namespace MosefakApp.Core.Dtos.Doctor.Responses
{
    public class DoctorProfileResponse
    {
        // Shared fields (Users table)
        public string FullName { get; set; } = null!;
        public string? ImagePath { get; set; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        // Doctor-specific fields (Doctors table)
        public List<SpecializationResponse> Specialty { get; set; } = new List<SpecializationResponse>();
        public int YearsOfExperience { get; set; }
        public string? AboutMe { get; set; }
        public double Rating { get; set; }
    }
}
