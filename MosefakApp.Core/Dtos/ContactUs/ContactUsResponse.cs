namespace MosefakApp.Core.Dtos.ContactUs
{
    public class ContactUsResponse
    {
        public string Id { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
