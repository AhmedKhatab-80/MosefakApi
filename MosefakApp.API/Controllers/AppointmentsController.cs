namespace MosefakApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentsService;

        public AppointmentsController(IAppointmentService appointmentsService)
        {
            _appointmentsService = appointmentsService;
        }

        [HttpGet("upcoming-appointments")]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>?>> UpcomingAppointments()
        {
            int userId = User.GetUserId();
            var query = await _appointmentsService.UpcomingAppointments(userId);

            return Ok(query);  
        }

        [HttpGet("canceled-appointments")]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>?>> CanceledAppointments()
        {
            int userId = User.GetUserId();
            var query = await _appointmentsService.CanceledAppointments(userId);

            return Ok(query);
        }

        [HttpGet("completed-appointments")]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>?>> CompletedAppointments()
        {
            int userId = User.GetUserId();
            var query = await _appointmentsService.CompletedAppointments(userId);

            return Ok(query);
        }

        [HttpPut("cancel-appointment")]
        public async Task<ActionResult<bool>> CancelAppointmentAsync(CancelAppointmentRequest request)
        {
            var query = await _appointmentsService.CancelAppointmentAsync(request.AppointmentId, request.CancelationReason);

            return Ok(query);
        }

        [HttpPut("reschedule-appointment")]
        public async Task<ActionResult<bool>> RescheduleAppointmentAsync(RescheduleAppointmentRequest request)
        {
            var query = await _appointmentsService.RescheduleAppointmentAsync(request.AppointmentId, request.NewDateTime);

            return Ok(query);
        }
    }
}
