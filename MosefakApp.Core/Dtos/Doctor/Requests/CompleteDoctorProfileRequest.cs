namespace MosefakApp.Core.Dtos.Doctor.Requests
{
    public class CompleteDoctorProfileRequest
    {
        public string LicenseNumber { get; set; } = null!;
        public string AboutMe { get; set; } = null!;
        public List<ClinicRequest> Clinics { get; set; } = new();
        public List<SpecializationRequest> Specializations { get; set; } = new();
        public List<AppointmentTypeRequest> AppointmentTypes { get; set; } = new();
        public List<EducationRequest> Educations { get; set; } = new();
        public List<ExperienceRequest> Experiences { get; set; } = new();
        public List<AwardRequest> Awards { get; set; } = new();
    }
}
