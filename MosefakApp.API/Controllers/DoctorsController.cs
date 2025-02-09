namespace MosefakApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Cached(duration: 600)] // 10 minutes
    [EnableRateLimiting(policyName: RateLimiterType.Concurrency)]
    public class DoctorsController : ApiBaseController
    {
        private readonly IDoctorService _doctorService;
        private readonly IIdProtectorService _idProtectorService;

        public DoctorsController(IDoctorService doctorService, IIdProtectorService idProtectorService)
        {
            _doctorService = doctorService;
            _idProtectorService = idProtectorService;
        }

        // ✅ Get all doctors
        [HttpGet]
        [HasPermission(Permissions.ViewDoctors)]
        public async Task<ActionResult<List<DoctorResponse>>> GetAllDoctors()
        {
            var doctors = await _doctorService.GetAllDoctors();

            doctors.ForEach(d => d.Id = ProtectId(d.Id));

            return Ok(doctors);
        }

        // ✅ Get doctor by ID
        [HttpGet("{doctorId}")]
        [HasPermission(Permissions.ViewDoctorById)]
        public async Task<ActionResult<DoctorResponse>> GetDoctorById(string doctorId)
        {
            var unprotectedId = UnprotectId(doctorId);
            if (unprotectedId == null) return BadRequest("Invalid doctor ID");

            var doctor = await _doctorService.GetDoctorById(unprotectedId.Value);

            doctor.Id = ProtectId(doctor.Id);
            return Ok(doctor);
        }

        // ✅ Get doctor profile (Authenticated Doctor)
        [HttpGet("profile")]
        [HasPermission(Permissions.ViewDoctorProfile)]
        public async Task<ActionResult<DoctorProfileResponse>> GetDoctorProfile()
        {
            var userId = User.GetUserId();

            var profile = await _doctorService.GetDoctorProfile(userId);

            profile.Id = ProtectId(userId.ToString());
            return Ok(profile);
        }

        // ✅ Get top 10 doctors
        [HttpGet("top-ten")]
        [HasPermission(Permissions.ViewTopTenDoctors)]
        public async Task<ActionResult<List<DoctorDto>>> GetTopTenDoctors()
        {
            var doctors = await _doctorService.TopTenDoctors();
            
            if (doctors is null)
                return Ok();

            doctors.ForEach(d => d.Id = ProtectId(d.Id));

            return Ok(doctors);
        }

        // ✅ Add a new doctor (For Admin)
        [HttpPost]
        [HasPermission(Permissions.CreateDoctor)]
        public async Task<IActionResult> AddDoctor(DoctorRequest request)
        {
            await _doctorService.AddDoctor(request);
            return CreatedAtAction(nameof(GetDoctorById), new { doctorId = request.AppUserId }, null);
        }

        // ✅ Complete doctor profile (For Doctor)
        [HttpPost("complete-profile")]
        [HasPermission(Permissions.CompleteDoctorProfile)]
        public async Task<IActionResult> CompleteDoctorProfile(CompleteDoctorProfileRequest request)
        {
            var userId = User.GetUserId();

            await _doctorService.CompleteDoctorProfile(userId, request);
            return Ok();
        }

        // ✅ Update doctor profile (For Doctor)
        [HttpPut("update-profile")]
        [HasPermission(Permissions.EditDoctorProfile)]
        public async Task<IActionResult> UpdateDoctorProfile([FromForm] DoctorProfileUpdateRequest request)
        {
            var userId = User.GetUserId();

            await _doctorService.UpdateDoctorProfile(request, userId);
            return Ok();
        }

        // ✅ Delete doctor (For Admin)
        [HttpDelete("{doctorId}")]
        [HasPermission(Permissions.DeleteDoctor)]
        public async Task<IActionResult> DeleteDoctor(string doctorId)
        {
            var unprotectedId = UnprotectId(doctorId);
            if (unprotectedId == null) return BadRequest("Invalid doctor ID");

            await _doctorService.DeleteDoctor(unprotectedId.Value);
            return NoContent();
        }

        // ✅ Get available time slots for a doctor
        [HttpGet("{doctorId}/available-timeslots")]
        [HasPermission(Permissions.ViewAvailableTimeSlots)]
        public async Task<ActionResult<List<DateTime>>> GetAvailableTimeSlots(string doctorId, [FromQuery] DateTime date, [FromQuery] int appointmentTypeId)
        {
            var unprotectedId = UnprotectId(doctorId);
            if (unprotectedId == null) return BadRequest("Invalid doctor ID");

            var timeSlots = await _doctorService.GetAvailableTimeSlots(unprotectedId.Value, date, appointmentTypeId);
            return Ok(timeSlots);
        }

        // ✅ Get appointment types for a doctor
        [HttpGet("{doctorId}/appointment-types")]
        [HasPermission(Permissions.ViewAppointmentTypes)]
        public async Task<ActionResult<List<AppointmentTypeResponse>>> GetAppointmentTypes(string doctorId)
        {
            var unprotectedId = UnprotectId(doctorId);
            if (unprotectedId == null) return BadRequest("Invalid doctor ID");

            var appointmentTypes = await _doctorService.GetAppointmentTypes(unprotectedId.Value);

            appointmentTypes.ForEach(a => a.Id = ProtectId(a.Id));
            return Ok(appointmentTypes);
        }

        // 🔥 Utility Methods for ID Protection
        private string ProtectId(string id) => _idProtectorService.Protect(int.Parse(id));
        private int? UnprotectId(string id) => _idProtectorService.UnProtect(id);
    }

}
