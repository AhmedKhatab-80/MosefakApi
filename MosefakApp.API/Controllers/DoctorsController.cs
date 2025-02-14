using MosefakApp.Core.Dtos.Period.Responses;
using MosefakApp.Core.Dtos.Schedule.Requests;

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
        public async Task<ActionResult<DoctorDetail>> GetDoctorById(string doctorId)
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
        public async Task<ActionResult<List<DoctorResponse>>> GetTopTenDoctors()
        {
            var doctors = await _doctorService.TopTenDoctors();
            
            if (doctors is null)
                return Ok();

            doctors.ForEach(d => d.Id = ProtectId(d.Id));

            return Ok(doctors);
        }

        [HttpGet("search-doctors")]
        public async Task<ActionResult<List<DoctorResponse>>> SearchDoctorsAsync([FromForm] DoctorSearchFilter filter)
        {
            var query = await _doctorService.SearchDoctorsAsync(filter);

            return Ok(query);
        }

        [HttpGet("upcoming-appointments")]
        [HasPermission(Permissions.ViewUpcomingAppointmentsForDoctor)]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>?> GetUpcomingAppointmentsAsync()
        {
            var userId = User.GetUserId();
            var query = await _doctorService.GetUpcomingAppointmentsAsync(userId);

            return Ok(query);
        }


        [HttpGet("past-appointments")]
        [HasPermission(Permissions.ViewPastAppointmentsForDoctor)]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>?> GetPastAppointmentsAsync()
        {
            var userId = User.GetUserId();
            var query = await _doctorService.GetPastAppointmentsAsync(userId);

            return Ok(query);
        }

        [HttpGet("total-appointments")]
        [HasPermission(Permissions.GetTotalAppointmentsAsync)]
        public async Task<ActionResult<long>> GetTotalAppointmentsAsync()
        {
            var doctorId = User.GetUserId();
            var query = await _doctorService.GetTotalAppointmentsAsync(doctorId);

            return Ok(query);
        }

        [HttpPost("upload-image")]
        [HasPermission(Permissions.UploadDoctorProfileImage)]
        public async Task<ActionResult<bool>> UploadProfileImageAsync(IFormFile imageFile, CancellationToken cancellationToken = default)
        {
            var userId = User.GetUserId();

            var query = await _doctorService.UploadProfileImageAsync(userId, imageFile, cancellationToken);

            return Ok(query);
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

        [HttpPut("update-working-times/{clinicId}")]
        public async Task<ActionResult<bool>> UpdateWorkingTimesAsync(int clinicId, IEnumerable<WorkingTimeRequest> workingTimes)
        {
            var userId = User.GetUserId();
            var query = await _doctorService.UpdateWorkingTimesAsync(userId,clinicId, workingTimes);    

            return Ok(query);
        }

        // ✅ Update doctor profile (For Doctor)
        [HttpPut("update-profile")]
        [HasPermission(Permissions.EditDoctorProfile)]
        public async Task<IActionResult> UpdateDoctorProfile([FromForm] DoctorProfileUpdateRequest request, CancellationToken cancellationToken = default)
        {
            var userId = User.GetUserId();

            await _doctorService.UpdateDoctorProfile(request, userId, cancellationToken);
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
        [HttpGet("{doctorId}/available-time-slots")]
        [HasPermission(Permissions.ViewAvailableTimeSlots)]
        public async Task<ActionResult<List<TimeSlot>>> GetAvailableTimeSlots(string doctorId,[FromQuery] int clinicId,[FromQuery] int appointmentTypeId,[FromQuery] DayOfWeek selectedDay)
        {
            var unprotectedId = UnprotectId(doctorId);
            if (unprotectedId == null) return BadRequest("Invalid doctor ID");

            var availableSlots = await _doctorService.GetAvailableTimeSlots(unprotectedId.Value, clinicId, appointmentTypeId, selectedDay);
            return Ok(availableSlots);
        }

        // 🔥 Utility Methods for ID Protection
        private string ProtectId(string id) => _idProtectorService.Protect(int.Parse(id));
        private int? UnprotectId(string id) => _idProtectorService.UnProtect(id);
    }

}
