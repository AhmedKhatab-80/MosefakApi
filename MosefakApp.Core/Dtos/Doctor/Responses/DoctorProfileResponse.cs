namespace MosefakApp.Core.Dtos.Doctor.Responses
{
    public class DoctorProfileResponse : UserProfileResponse
    {
        // Shared fields (Users table), I Inherited them from UserProfileResponse to avoid duplication

        // Doctor-specific fields (Doctors table)
        public List<SpecializationResponse> Specialty { get; set; } = new List<SpecializationResponse>();
        public int YearsOfExperience { get; set; }
        public string? AboutMe { get; set; }
        public double Rating { get; set; }
    }
}
