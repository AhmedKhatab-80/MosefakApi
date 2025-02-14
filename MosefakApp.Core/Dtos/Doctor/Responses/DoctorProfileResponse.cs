namespace MosefakApp.Core.Dtos.Doctor.Responses
{
    public class DoctorProfileResponse : UserProfileResponse
    {
        // Shared fields (Users table), I Inherited them from UserProfileResponse to avoid duplication

        // Doctor-specific fields (Doctors table)
        public string LicenseNumber { get; set; } = null!;
        public string AboutMe { get; set; } = null!;
        public int NumberOfReviews { get; set; }
        public int TotalYearsOfExperience { get; set; }
        public double Rating { get; set; }

        // 🔹 Specializations, Awards, Education, and Experiences
        public List<SpecializationResponse> Specializations { get; set; } = new();
        public List<AwardResponse> Awards { get; set; } = new();
        public List<EducationResponse> Education { get; set; } = new();
        public List<ExperienceResponse> Experiences { get; set; } = new();
    }
}
