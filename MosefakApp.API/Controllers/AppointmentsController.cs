namespace MosefakApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Cached(duration: 600)] // 10 minutes
    [EnableRateLimiting(policyName: RateLimiterType.Concurrency)]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentsService;
        public AppointmentsController(IAppointmentService appointmentsService)
        {
            _appointmentsService = appointmentsService;
        }

        [HttpGet("upcoming-appointments")]
        [HasPermission(Permissions.ViewUpcomingAppointments)]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>?>> UpcomingAppointments()
        {
            int userId = User.GetUserId();
            var query = await _appointmentsService.UpcomingAppointments(userId);

            return Ok(query);  
        }

        [HttpGet("canceled-appointments")]
        [HasPermission(Permissions.ViewCanceledAppointments)]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>?>> CanceledAppointments()
        {
            int userId = User.GetUserId();
            var query = await _appointmentsService.CanceledAppointments(userId);

            return Ok(query);
        }

        [HttpGet("completed-appointments")]
        [HasPermission(Permissions.ViewCompletedAppointments)]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>?>> CompletedAppointments()
        {
            int userId = User.GetUserId();
            var query = await _appointmentsService.CompletedAppointments(userId);

            return Ok(query);
        }

        [HttpPut("cancel-appointment")]
        [HasPermission(Permissions.CancelAppointment)]
        public async Task<ActionResult<bool>> CancelAppointmentAsync(CancelAppointmentRequest request)
        {
            var query = await _appointmentsService.CancelAppointmentAsync(request.AppointmentId, request.CancelationReason);

            return Ok(query);
        }

        [HttpPut("reschedule-appointment")]
        [HasPermission(Permissions.RescheduleAppointment)]
        public async Task<ActionResult<bool>> RescheduleAppointmentAsync(RescheduleAppointmentRequest request)
        {
            var query = await _appointmentsService.RescheduleAppointmentAsync(request.AppointmentId, request.NewDateTime);

            return Ok(query);
        }

        [HttpPost("Book-Appointment")]
        [HasPermission(Permissions.BookAppointment)]
        public async Task<ActionResult<bool>> BookAppointment(BookAppointmentRequest request)
        {
            int appUserId = User.GetUserId();

            var query = await _appointmentsService.BookAppointment(request, appUserId);

            return Ok(query); 
        }
    }
}
