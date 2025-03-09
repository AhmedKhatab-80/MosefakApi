namespace MosefakApp.Core.Dtos.User.Requests
{
    public class ChangeEmailRequest
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; } = null!;
    }
}
