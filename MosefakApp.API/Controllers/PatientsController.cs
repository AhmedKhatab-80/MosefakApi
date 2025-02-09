namespace MosefakApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Cached(duration: 600)] // 10 minutes
    [EnableRateLimiting(policyName: RateLimiterType.Concurrency)]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet("profile")]
        [HasPermission(Permissions.ViewPatientProfile)]
        public async Task<ActionResult<UserProfileResponse>> PatientProfile()
        {
            int userId = User.GetUserId();

            var response = await _patientService.PatientProfile(userId);

            return Ok(response);
        }

        [HttpPut]
        [HasPermission(Permissions.EditPatientProfile)]
        public async Task<ActionResult<UserProfileResponse>> UpdatePatientProfile([FromForm] UpdatePatientProfileRequest request)
        {
            int userId = User.GetUserId();

            var response = await _patientService.UpdatePatientProfile(userId, request);

            return Ok(response);
        }
    }
}
