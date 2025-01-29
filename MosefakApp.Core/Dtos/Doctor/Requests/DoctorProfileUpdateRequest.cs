namespace MosefakApp.Core.Dtos.Doctor.Requests
{
    public class DoctorProfileUpdateRequest
    {
        // Shared fields (Users table)
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? ImagePath { get; set; }
        public string PhoneNumber { get; set; } = null!;

        // Doctor-specific fields (Doctors table)
        public List<SpecializationResponse> specializations { get; set; } = new List<SpecializationResponse>();
        public int YearsOfExperience { get; set; }
        public string? IntroductionBreif { get; set; }
    }
}
