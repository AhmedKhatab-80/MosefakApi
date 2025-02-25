namespace MosefakApp.API.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    [Cached(duration: 600)] // 10 minutes
    [EnableRateLimiting(policyName: RateLimiterType.Concurrency)]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentsService;
        private readonly IIdProtectorService _idProtectorService;
        public AppointmentsController(IAppointmentService appointmentsService, IIdProtectorService idProtectorService)
        {
            _appointmentsService = appointmentsService;
            _idProtectorService = idProtectorService;
        }

        [HttpGet("patient")]
        [HasPermission(Permissions.ViewPatientAppointments)]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>?>> GetPatientAppointments([FromQuery]AppointmentStatus? status = null, CancellationToken cancellationToken = default)
        {
            int userId = User.GetUserId();
            var query = await _appointmentsService.GetPatientAppointments(userId, status, cancellationToken);

            query.ForEach(x => x.Id = ProtectId(x.Id));
            query.ForEach(x => x.DoctorId = ProtectId(x.DoctorId));
            query.ForEach(x => x.DoctorSpecialization.ForEach(s => s.Id = ProtectId(s.Id)));
            query.ForEach(x => x.AppointmentType.Id = ProtectId(x.AppointmentType.Id));

            return Ok(query);  
        }

        [HttpGet("{appointmentId}")]
        [HasPermission(Permissions.ViewAppointment)]
        public async Task<ActionResult<AppointmentResponse>> GetAppointmentById(string appointmentId, CancellationToken cancellationToken = default)
        {
            var unprotectedId = UnprotectId(appointmentId);

            if (unprotectedId == null)
                return BadRequest("Invalid Id");

            var query = await _appointmentsService.GetAppointmentById(unprotectedId.Value, cancellationToken);

            query.Id = appointmentId;
            query.DoctorId = ProtectId(query.DoctorId);
            query.AppointmentType.Id = ProtectId(query.AppointmentType.Id);
            query.DoctorSpecialization.ForEach(s => s.Id = ProtectId(s.Id));

            return Ok(query);
        }

        // Get Appointments by Date Range (Patient)
        [HttpGet("range")]
        [HasPermission(Permissions.ViewAppointmentsInRange)]
        public async Task<ActionResult<List<AppointmentResponse>>> GetAppointmentsByDateRange([FromQuery] DateTimeOffset startDate, [FromQuery] DateTimeOffset endDate, CancellationToken cancellationToken = default)
        {
            int userId = User.GetUserId();

            var query = await _appointmentsService.GetAppointmentsByDateRange(userId,startDate,endDate, cancellationToken);

            query.ForEach(x => x.Id = ProtectId(x.Id));
            query.ForEach(x => x.DoctorId = ProtectId(x.DoctorId));
            query.ForEach(x => x.DoctorSpecialization.ForEach(s => s.Id = ProtectId(s.Id)));
            query.ForEach(x => x.AppointmentType.Id = ProtectId(x.AppointmentType.Id));

            return Ok(query);
        }


        [HttpPut("{appointmentId}/approve")]
        [HasPermission(Permissions.ApproveAppointment)]
        public async Task<ActionResult<bool>> ApproveAppointmentByDoctor(string appointmentId)
        {
            var unprotectedId = UnprotectId(appointmentId);
            if (unprotectedId == null)
                return BadRequest("Invalid ID");
            var query = await _appointmentsService.ApproveAppointmentByDoctor(unprotectedId.Value);

            return Ok(query);
        }

        [HttpPut("{appointmentId}/reject")]
        [HasPermission(Permissions.RejectAppointment)]
        public async Task<ActionResult<bool>> RejectAppointmentByDoctor(string appointmentId, RejectAppointmentRequest request)
        {
            var unprotectedId = UnprotectId(appointmentId);
            if (unprotectedId == null)
                return BadRequest("Invalid ID");

            var query = await _appointmentsService.RejectAppointmentByDoctor(unprotectedId.Value, request.RejectionReason);

            return Ok(query);
        }

        [HttpPut("{appointmentId}/complete")]
        [HasPermission(Permissions.MarkAppointmentAsCompleted)]
        public async Task<ActionResult<bool>> MarkAppointmentAsCompleted(string appointmentId)
        {
            var unprotectedId = UnprotectId(appointmentId);
            if (unprotectedId == null)
                return BadRequest("Invalid ID");

            var query = await _appointmentsService.MarkAppointmentAsCompleted(unprotectedId.Value);

            return Ok(query);
        }

        [HttpDelete("{appointmentId}/doctor-cancel")]
        [HasPermission(Permissions.CancelAppointmentByDoctor)]
        public async Task<ActionResult<bool>> CancelAppointmentByDoctor(string appointmentId, CancelAppointmentRequest request)
        {
            var unprotectedId = UnprotectId(appointmentId);
            if (unprotectedId == null)
                return BadRequest("Invalid ID");

            var query = await _appointmentsService.CancelAppointmentByDoctor(unprotectedId.Value, request.CancelationReason);

            return Ok(query);
        }

        [HttpPut("{appointmentId}/patient-cancel")]
        [HasPermission(Permissions.CancelAppointmentByPatient)]
        public async Task<ActionResult<bool>> CancelAppointmentAsync(string appointmentId, CancelAppointmentRequest request, CancellationToken token = default)
        {
            var unprotectedId = UnprotectId(appointmentId);
            if (unprotectedId == null)
                return BadRequest("Invalid ID");

            int userId = User.GetUserId();

            var query = await _appointmentsService.CancelAppointmentByPatient(userId, unprotectedId.Value, request.CancelationReason, token);

            return Ok(query);
        }


        [HttpPost("{appointmentId}/pay")]
        [HasPermission(Permissions.PayForAppointment)]
        public async Task<ActionResult<bool>> Pay(string appointmentId, CancellationToken cancellationToken = default)
        {
            var unprotectedId = UnprotectId(appointmentId);
            if (unprotectedId == null)
                return BadRequest("Invalid ID");

            var query = await _appointmentsService.Pay(unprotectedId.Value, cancellationToken);

            return Ok(query);
        }

        [HttpPut("reschedule")]
        [HasPermission(Permissions.RescheduleAppointment)]
        public async Task<ActionResult<bool>> RescheduleAppointmentAsync(RescheduleAppointmentRequest request)
        {
            var unprotectedId = UnprotectId(request.AppointmentId);
            if (unprotectedId == null)
                return BadRequest("Invalid ID");

            var query = await _appointmentsService.RescheduleAppointmentAsync(unprotectedId.Value, request.selectedDate,request.newTimeSlot);

            return Ok(query);
        }

        [HttpPost("book")]
        [HasPermission(Permissions.BookAppointment)]
        public async Task<ActionResult<bool>> BookAppointment(BookAppointmentRequest request, CancellationToken cancellationToken = default)
        {
            var unprotectedDoctorId = UnprotectId(request.DoctorId);
            if (unprotectedDoctorId == null)
                return BadRequest("Invalid ID");

            var unprotectedAppointmentTypeId = UnprotectId(request.AppointmentTypeId);
            if (unprotectedAppointmentTypeId == null)
                return BadRequest("Invalid ID");

            int appUserId = User.GetUserId();

            request.DoctorId = unprotectedDoctorId.Value.ToString();
            request.AppointmentTypeId = unprotectedAppointmentTypeId.Value.ToString();
            var query = await _appointmentsService.BookAppointment(request, appUserId, cancellationToken);

            return Ok(query); 
        }

        [HttpGet("{appointmentId}/status")]
        [HasPermission(Permissions.ViewAppointmentStatus)]
        public async Task<ActionResult<AppointmentStatus>> GetAppointmentStatus(string appointmentId)
        {
            var unprotectedId = UnprotectId(appointmentId);
            if (unprotectedId == null)
                return BadRequest("Invalid ID");

            var query = await _appointmentsService.GetAppointmentStatus(unprotectedId.Value);

            return Ok(query);
        }

        [HttpGet("doctor")]
        [HasPermission(Permissions.ViewDoctorAppointments)]
        public async Task<ActionResult<List<AppointmentResponse>>> GetDoctorAppointments(AppointmentStatus? status = null, CancellationToken cancellationToken = default)
        {
            int doctorId = User.GetUserId();
            var query = await _appointmentsService.GetDoctorAppointments(doctorId, status, cancellationToken);

            query.ForEach(x => x.Id = ProtectId(x.Id));
            query.ForEach(x => x.DoctorId = ProtectId(x.DoctorId));
            query.ForEach(x => x.DoctorSpecialization.ForEach(s => s.Id = ProtectId(s.Id)));
            query.ForEach(x => x.AppointmentType.Id = ProtectId(x.AppointmentType.Id));

            return Ok(query);
        }

        [HttpGet("pending")]
        [HasPermission(Permissions.ViewPendingAppointmentsForDoctor)]
        public async Task<ActionResult<List<AppointmentResponse>>> GetPendingAppointmentsForDoctor(CancellationToken cancellationToken = default)
        {
            int doctorId = User.GetUserId();
            var query = await _appointmentsService.GetPendingAppointmentsForDoctor(doctorId, cancellationToken);

            query.ForEach(x => x.Id = ProtectId(x.Id));
            query.ForEach(x => x.DoctorId = ProtectId(x.DoctorId));
            query.ForEach(x => x.DoctorSpecialization.ForEach(s => s.Id = ProtectId(s.Id)));
            query.ForEach(x => x.AppointmentType.Id = ProtectId(x.AppointmentType.Id));

            return Ok(query);
        }

        [HttpGet("doctor/range")]
        [HasPermission(Permissions.ViewAppointmentsForDoctorInRange)]
        public async Task<ActionResult<List<AppointmentResponse>>> GetAppointmentsByDateRangeForDoctor([FromQuery]DateTimeOffset startDate, [FromQuery] DateTimeOffset endDate, CancellationToken cancellationToken = default)
        {
            int doctorId = User.GetUserId();
            var query = await _appointmentsService.GetAppointmentsByDateRangeForDoctor(doctorId,startDate,endDate, cancellationToken);

            query.ForEach(x => x.Id = ProtectId(x.Id));
            query.ForEach(x => x.DoctorId = ProtectId(x.DoctorId));
            query.ForEach(x => x.DoctorSpecialization.ForEach(s => s.Id = ProtectId(s.Id)));
            query.ForEach(x => x.AppointmentType.Id = ProtectId(x.AppointmentType.Id));

            return Ok(query);
        }

        // 🔥 Reusable Helper Method for ID Protection
        private int? UnprotectId(string protectedId) => _idProtectorService.UnProtect(protectedId);

        private string ProtectId(string id) => _idProtectorService.Protect(int.Parse(id));
    }
}
