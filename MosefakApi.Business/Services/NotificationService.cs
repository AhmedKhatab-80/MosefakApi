namespace MosefakApi.Business.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public NotificationService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<PaginatedResponse<NotificationResponse>> GetUserNotifications(
        int userId,
        CancellationToken cancellationToken = default,
        int page = 1,
        int pageSize = 10)
        {
            var (notifications, totalCount) = await _unitOfWork.Repository<Notification>()
                .GetAllAsync(x => x.UserId == userId, null, x => x.CreatedAt, true, page, pageSize);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (!notifications.Any()) // ✅ No need for `null` check
            {
                return new PaginatedResponse<NotificationResponse>
                {
                    Data = [],
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };
            }

            var userNames = await _userManager.Users
                .Where(x => notifications.Select(n => n.UserId).Contains(x.Id)) // ✅ Optimized lookup
                .ToDictionaryAsync(x => x.Id, x => $"{x.FirstName} {x.LastName}", cancellationToken);

            var response = notifications.Select(item => new NotificationResponse
            {
                CreatedAt = item.CreatedAt,
                Message = item.Message,
                Title = item.Title,
                FullNameUser = userNames.TryGetValue(item.UserId, out var fullName) ? fullName : "Unknown User"
            }).ToList();

            return new PaginatedResponse<NotificationResponse>
            {
                CurrentPage = page,
                Data = response,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }


        public async Task<bool> MarkNotificationAsRead(int userId, int notificationId, CancellationToken cancellationToken = default)
        {
            var notification = await _unitOfWork.Repository<Notification>()
                .FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId);

            if (notification == null)
                throw new ItemNotFound("Notification not found.");

            if (notification.IsRead)
                return true; // ✅ Prevents unnecessary DB writes

            notification.IsRead = true;

            var result = await _unitOfWork.CommitAsync(cancellationToken) > 0;
            return result;
        }

    }
}
