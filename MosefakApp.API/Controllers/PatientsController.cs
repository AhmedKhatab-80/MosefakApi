namespace MosefakApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileResponse>> PatientProfile()
        {
            int userId = User.GetUserId();

            var response = await _patientService.PatientProfile(userId);

            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<UserProfileResponse>> UpdatePatientProfile(UpdatePatientProfileRequest request)
        {
            int userId = User.GetUserId();

            var response = await _patientService.UpdatePatientProfile(userId, request);

            return Ok(response);
        }
    }
}
