namespace MosefakApp.Core.Dtos.User.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? ImagePath { get; set; } 
        public bool IsDisabled { get; set; }
        public IList<string> Roles { get; set; } = null!;
    }
}
