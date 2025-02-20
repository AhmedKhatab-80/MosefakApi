namespace MosefakApp.API.Controllers
{
    [Route("api/doctor/appointment-types")]
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
        [HttpGet]
        [HasPermission(Permissions.ViewAppointmentTypes)]
        public async Task<ActionResult<List<AppointmentTypeResponse>?>> GetAppointmentTypes()
        {
            var doctorId = User.GetUserId();

            var appointmentTypes = await _appointmentTypeService.GetAppointmentTypes(doctorId);

            if (appointmentTypes == null)
                return Ok();

            appointmentTypes.ForEach(a => a.Id = ProtectId(a.Id));
            return Ok(appointmentTypes);
        }

        [HttpPost]
        [HasPermission(Permissions.AddAppointmentTypes)]
        public async Task<ActionResult<bool>> AddAppointmentType(AppointmentTypeRequest request)
        {
            var doctorId = User.GetUserId();

            var result = await _appointmentTypeService.AddAppointmentType(doctorId, request);
            return result ? Ok(result) : BadRequest("Failed to add appointment type.");
        }

        [HttpPut("{protectedId}")]
        [HasPermission(Permissions.EditAppointmentTypes)]
        public async Task<ActionResult<bool>> EditAppointmentType(string protectedId, AppointmentTypeRequest request)
        {
            var unprotectedId = UnprotectId(protectedId);
            if (unprotectedId == null)
                return BadRequest("Invalid appointment type ID.");

            var result = await _appointmentTypeService.EditAppointmentType(unprotectedId.Value, request);
            return result ? Ok(result) : BadRequest("Failed to edit appointment type.");
        }

        [HttpDelete("{protectedId}")]
        [HasPermission(Permissions.DeleteAppointmentTypes)]
        public async Task<ActionResult<bool>> DeleteAppointmentType(string protectedId, CancellationToken cancellationToken)
        {
            var unprotectedId = UnprotectId(protectedId);
            if (unprotectedId == null)
                return BadRequest("Invalid appointment type ID.");

            var result = await _appointmentTypeService.DeleteAppointmentType(unprotectedId.Value);
            return result ? Ok(result) : BadRequest("Failed to delete appointment type.");
        }

        // 🔥 Utility Methods for ID Protection
        private string ProtectId(string id) => _idProtectorService.Protect(int.Parse(id));
        private int? UnprotectId(string id) => _idProtectorService.UnProtect(id);
    }
}
