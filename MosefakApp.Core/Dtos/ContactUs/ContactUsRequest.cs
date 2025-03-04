namespace MosefakApp.Core.Dtos.ContactUs
{
    public class ContactUsRequest
    {
        [Required]
        [MaxLength(500,ErrorMessage = Errors.MaxLength)]
        public string Message { get; set; } = null!;
    }
}
