namespace MosefakApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentTypesController : ControllerBase
    {
        private readonly IAppointmentTypeService _appointmentTypeService;
        private readonly IIdProtectorService _idProtectorService;
        public AppointmentTypesController(IAppointmentTypeService appointmentTypeService, IIdProtectorService idProtectorService)
        {
            _appointmentTypeService = appointmentTypeService;
            _idProtectorService = idProtectorService;
        }

        // ✅ Get appointment types for a doctor
        [HttpGet("{doctorId}/appointment-types")]
        [HasPermission(Permissions.ViewAppointmentTypes)]
        public async Task<ActionResult<List<AppointmentTypeResponse>>> GetAppointmentTypes(string doctorId)
        {
            var unprotectedId = UnprotectId(doctorId);
            if (unprotectedId == null) return BadRequest("Invalid doctor ID");

            var appointmentTypes = await _appointmentTypeService.GetAppointmentTypes(unprotectedId.Value);

            appointmentTypes.ForEach(a => a.Id = ProtectId(a.Id));
            return Ok(appointmentTypes);
        }


        // 🔥 Utility Methods for ID Protection
        private string ProtectId(string id) => _idProtectorService.Protect(int.Parse(id));
        private int? UnprotectId(string id) => _idProtectorService.UnProtect(id);
    }
}
