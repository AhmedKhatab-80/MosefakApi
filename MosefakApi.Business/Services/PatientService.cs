namespace MosefakApi.Business.Services
{
    public class PatientService : IPatientService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public PatientService(UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserProfileResponse?> PatientProfile(int userIdFromClaims)
        {
            var user = await _userManager.Users.Where(x => x.Id == userIdFromClaims)
                                               .Select(x => new UserProfileResponse
                                               {
                                                   Id = x.Id,
                                                   FirstName = x.FirstName,
                                                   LastName = x.LastName,
                                                   Email = x.Email!,
                                                   PhoneNumber = x.PhoneNumber,
                                                   Gender = x.Gender,
                                                   DateOfBirth = x.DateOfBirth,
                                                   Age = x.Age,
                                                   ImagePath = x.ImagePath,
                                                   Address = x.Address != null ? new AddressUserResponse
                                                   {
                                                       Id = x.Address.Id,
                                                       City = x.Address.City,
                                                       State = x.Address.State,
                                                       Street = x.Address.Street,
                                                       ZipCode = x.Address.ZipCode,
                                                   } :
                                                   null,
                                               })
                                               .FirstOrDefaultAsync();

            if (user is null)
                return null;

            return user;
        }

        public async Task<UserProfileResponse> UpdatePatientProfile(int userIdFromClaims, UpdatePatientProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userIdFromClaims.ToString());

            if (user is null)
                throw new ItemNotFound("user is not exist");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.DateOfBirth = request.DateOfBirth;
            user.Gender = request.Gender;

            // Handle Image Upload (Use IImageService Instead)
            //if (!string.IsNullOrEmpty(request.ImagePath))
            //{
            //    user.ImagePath = await _imageService.UploadImageAsync(request.ImagePath);
            //}

            user.ImagePath = request.ImagePath; // will optimize later, will use IImageService 

            user.Address ??= new Address();
            user.Address.State = request.Address.State;
            user.Address.City = request.Address.City;
            user.Address.Street = request.Address.Street;
            user.Address.ZipCode = request.Address.ZipCode;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var error = result.Errors.Select(x => x.Description).FirstOrDefault();
                throw new BadRequest(error);
            }

            var response = _mapper.Map<UserProfileResponse>(user);

            return response;
        }
    }
}
