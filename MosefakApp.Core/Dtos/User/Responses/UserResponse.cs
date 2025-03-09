namespace MosefakApp.Core.Dtos.User.Responses
{
    public class UserResponse
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsDisabled { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
