namespace MosefakApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Cached(duration: 600)]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [HasPermission(Permissions.GetAllRoles)]
        public async Task<IList<RoleResponse>> Get(bool IncludeDeleted = false)
        {
            return await _roleService.GetRolesAsync(IncludeDeleted);
        }

        [HttpGet("GetById/{id}")]
        [HasPermission(Permissions.GetRoleById)]
        public async Task<RoleResponse> GetById(int id)
        {
            return await _roleService.GetRoleByIdAsync(id);
        }

        [HttpPost]
        [HasPermission(Permissions.AddRole)]
        public async Task<RoleResponse> Add(RoleRequest request)
        {
            return await _roleService.AddRoleAsync(request);
        }

        [HttpPut("Edit/{id}")]
        [HasPermission(Permissions.EditRole)]
        public async Task<RoleResponse> Edit(int id, RoleRequest request)
        {
            return await _roleService.EditRoleAsync(id, request);
        }

        [HttpDelete("Delete/{id}")]
        [HasPermission(Permissions.DeleteRole)]
        public async Task<ActionResult> Delete(int id)
        {
            await _roleService.DeleteRoleAsync(id);

            return Ok();
        }
    }
}
