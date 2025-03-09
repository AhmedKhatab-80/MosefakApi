namespace MosefakApp.Core.IServices.FireBase
{
    public interface IFirebaseService
    {
        Task<bool> SendNotificationAsync(string fcmToken, string title, string message, CancellationToken cancellationToken = default);
    }
}
