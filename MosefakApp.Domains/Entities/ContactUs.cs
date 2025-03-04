namespace MosefakApp.Domains.Entities
{
    public class ContactUs : BaseEntity
    {
        public string Message { get; set; } = string.Empty;
        public int AppUserId { get; set; }
    }
}
