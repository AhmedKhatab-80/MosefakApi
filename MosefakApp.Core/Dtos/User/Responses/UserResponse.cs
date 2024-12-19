namespace MosefakApp.Core.Dtos.User.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsDisabled { get; set; }
        public IList<string> Roles { get; set; } = null!;
    }
}
