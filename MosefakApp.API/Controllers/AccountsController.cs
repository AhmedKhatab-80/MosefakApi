namespace MosefakApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IUserService _userService;
        public AccountsController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<UserProfileResponse> GetUserProfileAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return await _userService.GetUserProfileAsync(userId!);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserProfile(UpdateUserProfile updateUserProfile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _userService.UpdateUserProfile(userId!, updateUserProfile);

            return Ok();
        }

        [HttpPut("ChangePassword")]
        public async Task ChangePasswordAsync(ChangePasswordRequest changePasswordRequest)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _userService.ChangePasswordAsync(userId!, changePasswordRequest);
        }
    }
}
