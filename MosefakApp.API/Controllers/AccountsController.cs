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


        [HttpPut("ChangePassword")]
        public async Task ChangePasswordAsync(ChangePasswordRequest changePasswordRequest)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _userService.ChangePasswordAsync(userId!, changePasswordRequest);
        }
    }
}
