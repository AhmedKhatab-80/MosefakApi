namespace MosefakApi.Business.Services
{
    public class PatientService : IPatientService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IImageService _imageService;
        private readonly ICacheService _cacheService;
        private readonly ILoggerService _loggerService;
        private readonly IMapper _mapper;

        public PatientService(UserManager<AppUser> userManager, IImageService imageService, ICacheService cacheService, ILoggerService loggerService, IMapper mapper)
        {
            _userManager = userManager;
            _imageService = imageService;
            _cacheService = cacheService;
            _loggerService = loggerService;
            _mapper = mapper;
        }

        public async Task<UserProfileResponse?> PatientProfile(int userIdFromClaims)
        {
            var user = await _userManager.Users.Include(x=> x.Address)
                                               .Where(x => x.Id == userIdFromClaims)
                                               .FirstOrDefaultAsync();

            if (user is null)
                return null;

            var userProfile = _mapper.Map<UserProfileResponse>(user);

            return userProfile;
        }

        public async Task<UserProfileResponse> UpdatePatientProfile(int userIdFromClaims, UpdatePatientProfileRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userIdFromClaims.ToString());

            if (user is null)
            {
                _loggerService.LogWarning($"user is not exist with #ID: ", userIdFromClaims);
                throw new ItemNotFound("user is not exist");
            }

            var oldImagePath = user.ImagePath ?? string.Empty;
            string newImagePath = null;

            if (request.ImagePath is not null)
            {
                try
                {
                    newImagePath = await _imageService.UploadImageOnServer(
                        "Patient",
                        request.ImagePath,
                        deleteIfExist: false,
                        oldPath: oldImagePath,
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to upload image.", ex);
                }
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.DateOfBirth = request.DateOfBirth ?? user.DateOfBirth;
            user.Gender = request.Gender ?? user.Gender;
            user.ImagePath = newImagePath ?? oldImagePath;

            if (request.Address != null)
            {
                user.Address ??= new Address();
                user.Address.Country = request.Address.Country;
                user.Address.City = request.Address.City;
                user.Address.Street = request.Address.Street;
            }
                
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                // Clean up the newly uploaded image if the update fails
                if (newImagePath != null)
                    await _imageService.RemoveImage($"Patient/{newImagePath}");

                // Return all validation errors
                var errors = result.Errors.Select(e => e.Description).ToList();
                throw new BadRequest($"Failed to update profile because {string.Join(',', errors)}" );
            }

            // Clean up the old image if a new one was uploaded
            if (newImagePath != null && !string.IsNullOrEmpty(oldImagePath))
                await _imageService.RemoveImage($"Patient/{oldImagePath}");

            // Map and return the response
            var response = _mapper.Map<UserProfileResponse>(user);

            if (response == null)
                throw new InvalidOperationException("Failed to map user profile.");

            await _cacheService.RemoveCachedResponseAsync("/api/Paitents/profile");

            return response;
        }
    }
}
