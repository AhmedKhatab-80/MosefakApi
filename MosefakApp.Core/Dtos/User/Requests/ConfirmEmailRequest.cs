namespace MosefakApp.Core.Dtos.User.Requests
{
    public class ConfirmEmailRequest
    {
        [Required]
        public int userId { get; set; } // will recieve it from Front-end

        [Required]
        public string Code { get; set; } = null!;
    }
}
