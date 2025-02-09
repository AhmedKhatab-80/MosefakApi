namespace MosefakApp.Core.Dtos.Doctor.Requests
{
    public class DoctorProfileUpdateRequest 
    {
        // Shared fields (Users table)
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public IFormFile? ImagePath { get; set; }
        public string? PhoneNumber { get; set; }
        public Gender? Gender { get; set; }
        public AddressUserRequest? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Doctor-specific fields (Doctors table)
        public List<SpecializationRequest> Specializations { get; set; } = new List<SpecializationRequest>();
        public List<AppointmentTypeRequest> AppointmentTypes { get; set; } = new List<AppointmentTypeRequest>();
        public List<WorkingTimeRequest> WorkingTimes { get; set; } = new List<WorkingTimeRequest>();
        public List<ClinicAddressRequest> ClinicAddresses { get; set; } = new List<ClinicAddressRequest>();
        public int YearsOfExperience { get; set; }
        public string? AboutMe { get; set; }
        public string LicenseNumber { get; set; } = null!;

    }
}
