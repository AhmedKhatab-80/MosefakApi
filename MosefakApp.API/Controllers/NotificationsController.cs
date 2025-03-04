namespace MosefakApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IFirebaseService _firebaseService;
        private readonly UserManager<AppUser> _userManager;
        private readonly INotificationService _notificationService;
        public NotificationsController(IFirebaseService firebaseService, UserManager<AppUser> userManager, INotificationService notificationService)
        {
            _firebaseService = firebaseService;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationDto model,CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null || string.IsNullOrEmpty(user.FcmToken))
                return NotFound("User or FCM Token not found");

            var success = await _firebaseService.SendNotificationAsync(user.FcmToken, model.Title, model.Message, cancellationToken);
            return success ? Ok("Notification sent!") : StatusCode(500, "Error sending notification");
        }

        [HttpGet("get-user-notifications")]
        [Authorize] // Ensure only logged-in users can access
        public async Task<ActionResult<PaginatedResponse<NotificationResponse>>> GetUserNotifications(int userId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = await _notificationService.GetUserNotifications(userId, cancellationToken, page, pageSize);

            return Ok(query);
        }

        [HttpPost("mark-as-read/{notificationId}")]
        [Authorize]
        public async Task<ActionResult<bool>> MarkNotificationAsRead(int notificationId, CancellationToken cancellationToken = default)
        {
            var userId = User.GetUserId();

            var query = await _notificationService.MarkNotificationAsRead(userId, notificationId, cancellationToken);

            return Ok(query);
        }

    }
}
