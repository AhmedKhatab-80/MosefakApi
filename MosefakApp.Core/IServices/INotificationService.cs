namespace MosefakApp.Core.IServices
{
    public interface INotificationService
    {
        Task<PaginatedResponse<NotificationResponse>> GetUserNotifications(int userId, CancellationToken cancellationToken = default, int page = 1, int pageSize = 10);
        Task<bool> MarkNotificationAsRead(int userId, int notificationId, CancellationToken cancellationToken = default);
    }
}
