namespace MosefakApp.Core.Dtos.Notification
{
    public class NotificationResponse
    {
        public string FullNameUser { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
