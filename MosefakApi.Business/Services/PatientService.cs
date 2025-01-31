
namespace MosefakApi.Business.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public PatientService(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<UserProfileResponse> PatientProfile(int userIdFromClaims)
        {
            var user = await _userService.GetUserProfileAsync(userIdFromClaims.ToString());

            if(user == null) 
                return new UserProfileResponse();

            var response = new UserProfileResponse()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                ImagePath = user.ImagePath,
                Age = user.Age,
            };

            return response;
        }

        public async Task<UserProfileResponse> UpdatePatientProfile(int userIdFromClaims, UpdatePatientProfileRequest request)
        {
            var user = await _userService.GetUserByIdAsync(userIdFromClaims);

            if (user == null)
                throw new ItemNotFound();

            var response = await _userService.UpdateUserProfile(userIdFromClaims, request);
            response.Email = user.Email;

            return response;
        }
    }
}
