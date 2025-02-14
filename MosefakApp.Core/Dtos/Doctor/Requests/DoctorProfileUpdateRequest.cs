namespace MosefakApp.Core.Dtos.Doctor.Requests
{
    public class DoctorProfileUpdateRequest 
    {
        // Shared fields (Users table)
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public Gender? Gender { get; set; }
        public AddressUserRequest? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Doctor-specific fields (Doctors table)
        public string? AboutMe { get; set; }
        public string LicenseNumber { get; set; } = null!;
    }
}
