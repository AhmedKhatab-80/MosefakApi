namespace MosefakApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Cached(duration: 600)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [HasPermission(Permissions.GetAllUsers)]
        public async Task<IList<UserResponse>> Get(bool IncludeDeleted = false)
        {
            return await _userService.GetUsersAsync(IncludeDeleted);
        }

        [HttpGet("GetById/{id}")]
        [HasPermission(Permissions.GetUserById)]
        public async Task<UserResponse> GetById(int id)
        {
            return await _userService.GetUserByIdAsync(id);
        }

        [HttpPost]
        [HasPermission(Permissions.AddUser)]
        public async Task<UserResponse> Add(UserRequest user)
        {
            return await _userService.CreateUserAsync(user);
        }

        [HttpPut("{id}")]
        [HasPermission(Permissions.EditUser)]
        public async Task<UserResponse> Update(int id, UserRequest user)
        {
            return await _userService.UpdateUserAsync(id, user);
        }

        [HttpDelete("{id}")]
        [HasPermission(Permissions.DeleteUser)]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);

            return Ok();
        }

        [HttpPut("UnLock/{id}")]
        [HasPermission(Permissions.UnLockUser)]
        public async Task<IActionResult> UnLock(int id)
        {
            await _userService.UnLock(id);

            return Ok();
        }
    }
}
