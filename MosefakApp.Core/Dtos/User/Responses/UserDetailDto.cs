namespace MosefakApp.Core.Dtos.User.Responses
{
    public class UserDetailDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
