namespace MosefakApp.Core.Dtos.User.Requests
{
    public class UpdatePatientProfileRequest
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? ImagePath { get; set; }
        public string? PhoneNumber { get; set; }
        public Gender? Gender { get; set; }
        public AddressUserResponse Address { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
    }
}
