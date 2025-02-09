namespace MosefakApp.Core.Dtos.Doctor.Responses
{
    public class DoctorResponse
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
        public int YearOfExperience { get; set; }
        public string LicenseNumber { get; set; } = null!;
        public string AboutMe { get; set; } = null!;
        public IList<ClinicAddressResponse> ClinicAddresses { get; set; } = new List<ClinicAddressResponse>();
        public IList<SpecializationResponse> Specializations { get; set; } = new List<SpecializationResponse>();
        public ICollection<ReviewResponse> Reviews { get; set; } = new List<ReviewResponse>();
        public ICollection<WorkingTimeResponse> WorkingTimes { get; set; } = new List<WorkingTimeResponse>();
        public int NumberOfReviews { get; set; }
        public IList<AppointmentTypeResponse> AppointmentTypes { get; set; } = new List<AppointmentTypeResponse>();

    }
}
