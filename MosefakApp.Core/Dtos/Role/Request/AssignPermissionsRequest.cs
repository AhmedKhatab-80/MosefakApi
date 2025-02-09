namespace MosefakApp.Core.Dtos.Role.Request
{
    public class AssignPermissionsRequest
    {
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
