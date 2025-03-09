namespace MosefakApp.Core.Dtos.Notification
{
    public class SendNotificationDto
    {
        public string UserId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
