namespace MosefakApp.Core.Dtos.User.Responses
{
    public class UserProfileResponse
    {
        public string Id { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; } 
        public string? ImageUrl { get; set; }
        public Gender? Gender { get; set; }
        public AddressUserResponse? Address { get; set; } 
        public DateTime? DateOfBirth { get; set; }
        public int Age { get; set; }
    }
}
