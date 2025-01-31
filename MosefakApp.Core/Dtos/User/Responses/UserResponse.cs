namespace MosefakApp.Core.Dtos.User.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsDisabled { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
