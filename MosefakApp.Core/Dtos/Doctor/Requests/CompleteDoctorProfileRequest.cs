namespace MosefakApp.Core.Dtos.Doctor.Requests
{
    public class CompleteDoctorProfileRequest
    {
        public int YearOfExperience { get; set; }
        public string LicenseNumber { get; set; } = null!;
        public string AboutMe { get; set; } = null!;
        public IList<ClinicAddressRequest> ClinicAddresses { get; set; } = new List<ClinicAddressRequest>();
        public IList<SpecializationRequest> Specializations { get; set; } = new List<SpecializationRequest>();
        public ICollection<WorkingTimeRequest> WorkingTimes { get; set; } = new List<WorkingTimeRequest>();
        public ICollection<AppointmentTypeRequest> AppointmentTypes { get; set; } = new List<AppointmentTypeRequest>();
    }
}
