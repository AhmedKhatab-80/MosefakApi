namespace MosefakApp.Core.Dtos.Doctor.Responses
{
    public class DoctorProfileResponse : UserProfileResponse
    {
        // Shared fields (Users table), I Inherited them from UserProfileResponse to avoid duplication

        // Doctor-specific fields (Doctors table)
        public int YearsOfExperience { get; set; }
        public string? AboutMe { get; set; }
        public double Rating { get; set; }
        public List<SpecializationResponse> Specialty { get; set; } = new List<SpecializationResponse>();
        public List<AppointmentTypeResponse> AppointmentTypes { get; set; } = new List<AppointmentTypeResponse>();
        public List<WorkingTimeResponse> WorkingTimes { get; set; } = new List<WorkingTimeResponse>();
        public IList<ClinicAddressResponse> ClinicAddresses { get; set; } = new List<ClinicAddressResponse>();
    }
}
