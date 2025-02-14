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

        [HttpPut("{appointmentId}/approve")]
        [HasPermission(Permissions.ApproveAppointment)]
        public async Task<ActionResult<bool>> ApproveAppointmentByDoctor(int appointmentId)
        {
            var query = await _appointmentsService.ApproveAppointmentByDoctor(appointmentId);

            return Ok(query);
        }

        [HttpPut("{appointmentId}/reject")]
        [HasPermission(Permissions.RejectAppointment)]
        public async Task<ActionResult<bool>> RejectAppointmentByDoctor(int appointmentId, RejectAppointmentRequest request)
        {
            var query = await _appointmentsService.RejectAppointmentByDoctor(appointmentId, request.RejectionReason);

            return Ok(query);
        }

        [HttpPut("{appointmentId}/complete")]
        [HasPermission(Permissions.MarkAppointmentAsCompleted)]
        public async Task<ActionResult<bool>> MarkAppointmentAsCompleted(int appointmentId)
        {
            var query = await _appointmentsService.MarkAppointmentAsCompleted(appointmentId);

            return Ok(query);
        }

        [HttpDelete("{appointmentId}/doctor-cancel")]
        [HasPermission(Permissions.CancelAppointmentByDoctor)]
        public async Task<ActionResult<bool>> CancelAppointmentByDoctor(int appointmentId, CancelAppointmentRequest request)
        {
            var query = await _appointmentsService.CancelAppointmentByDoctor(appointmentId, request.CancelationReason);

            return Ok(query);
        }

        [HttpPut("{appointmentId}/patient-cancel")]
        [HasPermission(Permissions.CancelAppointmentByPatient)]
        public async Task<ActionResult<bool>> CancelAppointmentAsync(int appointmentId, CancelAppointmentRequest request)
        {
            var query = await _appointmentsService.CancelAppointmentByPatient(appointmentId, request.CancelationReason);

            return Ok(query);
        }

        [HttpPost("auto-cancel")]
        public async Task<ActionResult> AutoCancelUnpaidAppointments()
        {
            await _appointmentsService.AutoCancelUnpaidAppointments();

            return Ok();
        }

        [HttpPost("{appointmentId}/pay")]
        [HasPermission(Permissions.PayForAppointment)]
        public async Task<ActionResult<bool>> Pay(int appointmentId)
        {
            var query = await _appointmentsService.Pay(appointmentId);

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
