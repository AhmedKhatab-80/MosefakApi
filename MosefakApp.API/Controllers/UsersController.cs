namespace MosefakApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Cached(duration: 600)] // 600/60 = 10 minutes
    [EnableRateLimiting(policyName: RateLimiterType.Concurrency)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IIdProtectorService _idProtectorService;

        public UsersController(IUserService userService, IIdProtectorService idProtectorService)
        {
            _userService = userService;
            _idProtectorService = idProtectorService;
        }

        // ✅ Get All Users
        [HttpGet]
        [HasPermission(Permissions.Users.View)]
        public async Task<ActionResult<IList<UserResponse>>> GetAllUsers([FromQuery] bool includeDeleted = false)
        {
            var users = await _userService.GetUsersAsync(includeDeleted);

            // Protect IDs in the response
            users.ForEach(user => user.Id = ProtectId(user.Id));

            return Ok(users);
        }

        // ✅ Get User By Encrypted ID
        [HttpGet("{id}")]
        [HasPermission(Permissions.Users.ViewById)]
        public async Task<ActionResult<UserResponse>> GetById(string id)
        {
            var unprotectedId = UnprotectId(id);
            if (unprotectedId == null) return BadRequest("Invalid user ID");

            var user = await _userService.GetUserByIdAsync(unprotectedId.Value);

            user.Id = ProtectId(user.Id);
            return Ok(user);
        }

        // ✅ Add User
        [HttpPost]
        [HasPermission(Permissions.Users.Create)]
        public async Task<ActionResult<UserResponse>> AddUser(UserRequest user)
        {
            var newUser = await _userService.CreateUserAsync(user);
            newUser.Id = ProtectId(newUser.Id);
            return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);
        }

        // ✅ Update User
        [HttpPut("{id}")]
        [HasPermission(Permissions.Users.Edit)]
        public async Task<ActionResult<UserResponse>> UpdateUser(string id, UserRequest user)
        {
            var unprotectedId = UnprotectId(id);

            if (unprotectedId == null) 
                return BadRequest("Invalid user ID");

            var updatedUser = await _userService.UpdateUserAsync(unprotectedId.Value, user);
            updatedUser.Id = ProtectId(updatedUser.Id);

            return Ok(updatedUser);
        }

        // ✅ Delete User
        [HttpDelete("{id}")]
        [HasPermission(Permissions.Users.Delete)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var unprotectedId = UnprotectId(id);

            if (unprotectedId == null) 
                return BadRequest("Invalid user ID");

            await _userService.DeleteUserAsync(unprotectedId.Value);
            return NoContent();
        }

        // ✅ Unlock User
        [HttpPut("unlock/{id}")]
        [HasPermission(Permissions.Users.UnLock)]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var unprotectedId = UnprotectId(id);

            if (unprotectedId == null) 
                return BadRequest("Invalid user ID");

            await _userService.UnLock(unprotectedId.Value);
            return Ok();
        }

        // 🔥 Reusable Helper Method for ID Protection
        private int? UnprotectId(string protectedId) => _idProtectorService.UnProtect(protectedId);

        private string ProtectId(string id) => _idProtectorService.Protect(int.Parse(id));
    }

}
