namespace MosefakApp.Core.Dtos.Role.Request
{
    public class RoleRequest
    {
        [Required]
        [RegularExpression(RegexPatterns.NamePattern, ErrorMessage = Errors.NameRegExp)]
        public string Name { get; set; } = null!;
        public IList<string> Permissions { get; set; } = new List<string>();
    }
}
