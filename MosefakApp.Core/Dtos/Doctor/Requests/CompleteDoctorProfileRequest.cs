namespace MosefakApp.Core.Dtos.Doctor.Requests
{
    public class CompleteDoctorProfileRequest
    {
        public string LicenseNumber { get; set; } = null!;
        public string AboutMe { get; set; } = null!;
       
        [Required]
        public List<ClinicRequest> Clinics { get; set; } = new();
        
        [Required]
        public List<SpecializationRequest> Specializations { get; set; } = new();
        
        [Required]
        public List<AppointmentTypeRequest> AppointmentTypes { get; set; } = new();
        // ✅ Optional Sections (Can be added later)
        public List<AwardRequest>? Awards { get; set; }
        public List<EducationRequest>? Educations { get; set; }
        public List<ExperienceRequest>? Experiences { get; set; }
    }
}
