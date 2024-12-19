namespace MosefakApp.Core.Dtos.Role.Responses
{
    public class RoleResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public IList<string> Permissions { get; set; } = null!;
    }
}
