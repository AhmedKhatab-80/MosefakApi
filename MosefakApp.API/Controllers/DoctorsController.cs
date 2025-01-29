namespace MosefakApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ApiBaseController
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<ActionResult<IList<DoctorResponse>>> GetAllDoctors()
        {
            var query = await _doctorService.GetAllDoctors();

            return Ok(query); 
        }

        [HttpGet("{doctorId}")]
        public async Task<ActionResult<DoctorResponse>> GetDoctorById(int doctorId)
        {
            var query = await _doctorService.GetDoctorById(doctorId);

            return Ok(query);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<DoctorProfileResponse>> GetDoctorProfile()
        {
            var userId = User.GetUserId();

            var query = await _doctorService.GetDoctorProfile(userId);

            return Ok(query);
        }

        [HttpGet("Top-Ten-Doctors")]
        public async Task<ActionResult<IList<DoctorDto>>> TopTenDoctors()
        {
            var query = await _doctorService.TopTenDoctors();

            return Ok(query);
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor(DoctorRequest request)
        {
            await _doctorService.AddDoctor(request);

            return Ok();
        }

        [HttpPost("Complete-Doctor-Profile")]
        public async Task<IActionResult> CompleteDoctorProfile(CompleteDoctorProfileRequest doctor)
        {
            var userId = User.GetUserId();

            await _doctorService.CompleteDoctorProfile(userId, doctor);

            return Ok();
        }

        [HttpPut("Update-Doctor-Profile")]
        public async Task<IActionResult> UpdateDoctorProfile(DoctorProfileUpdateRequest request)
        {
            var userId = User.GetUserId();
            
            await _doctorService.UpdateDoctorProfile(request, userId);

            return Ok();
        }

        [HttpDelete("{doctorId}")]
        public async Task<IActionResult> DeleteDoctor(int doctorId)
        {
            await _doctorService.DeleteDoctor(doctorId);

            return Ok();
        }
    }
}
