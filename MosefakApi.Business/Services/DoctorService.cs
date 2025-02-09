namespace MosefakApi.Business.Services
{
    public class DoctorService : IDoctorService
    {

        private readonly IUnitOfWork<Doctor> _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;
        private readonly IImageService _imageService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly string _baseUrl;

        public DoctorService(IUnitOfWork<Doctor> unitOfWork, IUserRepository userRepository, ICacheService cacheService, IImageService imageService, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _cacheService = cacheService;
            _imageService = imageService;
            _configuration = configuration;
            _mapper = mapper;
            _baseUrl = _configuration["BaseUrl"] ?? "https://default-url.com/";
        }

        public async Task<List<DoctorResponse>> GetAllDoctors()
        {
            var doctors = await _unitOfWork.DoctorRepositoryAsync.GetDoctors();

            if (doctors == null || !doctors.Any())
                return new List<DoctorResponse>(); // Return an empty list instead of throwing an exception

            // Extract AppUser IDs for doctors
            var appUserIds = doctors.Select(d => d.AppUserId).ToHashSet();

            // Extract unique reviewer IDs (avoid duplicate IDs)
            var reviewerIds = doctors.SelectMany(d => d.Reviews.Select(r => r.AppUserId)).ToHashSet();

            // Fetch user details for doctors & reviewers in a **single batch request**
            var userDetails = await _unitOfWork.DoctorRepositoryAsync.GetUserDetailsAsync(appUserIds.Union(reviewerIds));

            // Map the results
            return doctors.Select(d => MapDoctorResponse(d, userDetails)).ToList();
        }


        public async Task<DoctorResponse> GetDoctorById(int doctorId)
        {
            var doctor = await _unitOfWork.DoctorRepositoryAsync.FirstOrDefaultASync(x=> x.Id == doctorId, ["AppointmentTypes", "Reviews", "ClinicAddresses", "Specializations", "WorkingTimes"]);

            if (doctor is null)
                throw new ItemNotFound("Doctor is not exist");

            var reviewerIds = doctor.Reviews.Select(x => x.AppUserId).ToHashSet();
            reviewerIds.Add(doctor.AppUserId);

            var userDetails = await _unitOfWork.DoctorRepositoryAsync.GetUserDetailsAsync(reviewerIds);

            // Map the results
            return MapDoctorResponse(doctor, userDetails);
        }

        public async Task<List<DoctorDto>?> TopTenDoctors()
        {
            var query = await _unitOfWork.DoctorRepositoryAsync.GetTopTenDoctors();

            if (query is null)
                return new List<DoctorDto>();

            return query;
        }

        public async Task AddDoctor(DoctorRequest request)
        {
            if (request is null)
                throw new BadRequest("Data is null");

            // check if AppUserId is exist or not

            var user = await _userRepository.GetUserByIdAsync(request.AppUserId);

            if (user is null)
                throw new ItemNotFound("this doctor is not registered!");

            var doctor = _mapper.Map<Doctor>(request);

            await _unitOfWork.DoctorRepositoryAsync.AddEntityAsync(doctor);
            await _unitOfWork.CommitAsync();


            // ✅ Remove relevant cache entries after adding a doctor
            await RemoveCachedDoctorData(doctor.Id);
        }

        public async Task CompleteDoctorProfile(int appUserIdFromClaims, CompleteDoctorProfileRequest doctor)
        {
            var user = await _userRepository.GetUserByIdAsync(appUserIdFromClaims);

            if (user is null)
                throw new ItemNotFound("this doctor is not registered!");

            var doc = _mapper.Map<Doctor>(doctor);
            doc.AppUserId = appUserIdFromClaims;

            await _unitOfWork.DoctorRepositoryAsync.AddEntityAsync(doc);
            await _unitOfWork.CommitAsync();


            // ✅ Remove relevant cache entries after complete a doctor profile
            await RemoveCachedDoctorData(doc.Id);
        }

        public async Task<DoctorProfileResponse> GetDoctorProfile(int appUserIdFromClaims)
        {
            var profile = await _unitOfWork.DoctorRepositoryAsync.GetDoctorProfile(appUserIdFromClaims);

            if (profile is null)
                return new DoctorProfileResponse();

            return profile;
        }

        public async Task UpdateDoctorProfile(DoctorProfileUpdateRequest request, int appUserIdFromClaims, CancellationToken cancellationToken = default)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled); // beacuse EF Core doesn't Rollback for different Databases

            // 1️) Fetch User from Identity Database
            var user = await _userRepository.GetUserByIdAsync(appUserIdFromClaims);
            if (user is null)
                throw new ItemNotFound("This doctor is not registered!");

            // 2️) Fetch Doctor from Business Database
            var doctor = await _unitOfWork.DoctorRepositoryAsync.FirstOrDefaultASync(x => x.AppUserId == appUserIdFromClaims);
            if (doctor is null)
                throw new ItemNotFound("This profile does not exist!");

            // 3️) Update User Properties
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.DateOfBirth = request.DateOfBirth ?? user.DateOfBirth;
            user.Gender = request.Gender ?? user.Gender;

            if (request.Address != null)
            {
                user.Address ??= new Address();
                user.Address.State = request.Address.State;
                user.Address.City = request.Address.City;
                user.Address.Street = request.Address.Street;
                user.Address.ZipCode = request.Address.ZipCode;
            }

            // 4️) Handle Image Updates
            var oldImagePath = user.ImagePath ?? string.Empty;
            string? newImagePath = null;

            if (request.ImagePath is not null)
            {
                newImagePath = await _imageService.UploadImageOnServer("Doctor", request.ImagePath, false, oldImagePath, cancellationToken);
            }

            // 5️) Update Doctor Properties
            doctor.AboutMe = request.AboutMe ?? doctor.AboutMe;
            doctor.YearOfExperience = request.YearsOfExperience;
            doctor.LicenseNumber = request.LicenseNumber ?? doctor.LicenseNumber;

            // ✅ **Optimize List Updates (No Need for Manual Loops)**
            doctor.Specializations = request.Specializations?.Select(s => new Specialization { Name = s.Name, Category = s.Category }).ToList() ?? doctor.Specializations;
            doctor.WorkingTimes = request.WorkingTimes?.Select(w => new WorkingTime { StartTime = w.StartTime, EndTime = w.EndTime, DayOfWeek = w.DayOfWeek }).ToList() ?? doctor.WorkingTimes;
            doctor.ClinicAddresses = request.ClinicAddresses?.Select(c => new ClinicAddress { Street = c.Street, City = c.City, Country = c.Country }).ToList() ?? doctor.ClinicAddresses;
            doctor.AppointmentTypes = request.AppointmentTypes?.Select(a => new AppointmentType { ConsultationFee = a.ConsultationFee, VisitType = a.VisitType, Duration = a.Duration }).ToList() ?? doctor.AppointmentTypes;

            try
            {
                // 6️) Save Changes in Transaction Scope
                await _userRepository.UpdateUser(user);
                var rowsAffected = await _userRepository.Save();

                if (rowsAffected <= 0)
                {
                    if (!string.IsNullOrEmpty(newImagePath))
                        await _imageService.RemoveImage($"Doctor/{newImagePath}");

                    throw new BadRequest("Failed to update profile");
                }

                if (!string.IsNullOrEmpty(newImagePath) && !string.IsNullOrEmpty(oldImagePath))
                    await _imageService.RemoveImage($"Doctor/{oldImagePath}");

                await _unitOfWork.DoctorRepositoryAsync.UpdateEntityAsync(doctor);
                await _unitOfWork.CommitAsync();

                transactionScope.Complete();

                // ✅ Remove relevant cache entries after updating a doctor
                await RemoveCachedDoctorData(doctor.Id);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update doctor profile", ex);
            }
        }


        public async Task DeleteDoctor(int doctorId)
        {
            var doctor = await _unitOfWork.DoctorRepositoryAsync.GetByIdAsync(doctorId);

            if (doctor is null)
                throw new ItemNotFound("Doctor is not exist");

            await _unitOfWork.DoctorRepositoryAsync.DeleteEntityAsync(doctor);
            await _unitOfWork.CommitAsync();

            // ✅ Remove relevant cache entries after deleting a doctor
            await RemoveCachedDoctorData(doctorId);
        }


        public async Task<List<DateTime>> GetAvailableTimeSlots(int doctorId, DateTime date, int appointmentTypeId)
        {
            var query = await _unitOfWork.DoctorRepositoryAsync.GetAvailableTimeSlots(doctorId, date, appointmentTypeId);

            if (query is null)
                return new List<DateTime>();

            return query;
        }

        public async Task<List<AppointmentTypeResponse>> GetAppointmentTypes(int doctorId)
        {
            var query = await _unitOfWork.DoctorRepositoryAsync.GetAppointmentTypes(doctorId);

            if(query is null)
                return new List<AppointmentTypeResponse>();

            var response = _mapper.Map<List<AppointmentTypeResponse>>(query);

            return response;
        }

        // 🔥 **Only removes cache; doesn't generate or set cache keys**
        private async Task RemoveCachedDoctorData(int doctorId)
        {
            string _base = "/api/doctors";

            await _cacheService.RemoveCachedResponseAsync(_base);
            await _cacheService.RemoveCachedResponseAsync($"{_base}/{doctorId}");
            await _cacheService.RemoveCachedResponseAsync($"{_base}/top-ten");
            await _cacheService.RemoveCachedResponseAsync($"{_base}/{doctorId}/available-timeslots");
            await _cacheService.RemoveCachedResponseAsync($"{_base}/{doctorId}/appointment-types");
        }

        /// <summary>
        /// Maps a Doctor entity to a DoctorResponse DTO efficiently.
        /// </summary>
        private DoctorResponse MapDoctorResponse(
            Doctor doctor,
            Dictionary<int, (string FullName, string ImagePath)> userDetails)
        {
            var doctorFullName = userDetails.TryGetValue(doctor.AppUserId, out var doctorInfo)
                ? doctorInfo.FullName
                : "Unknown";

            var doctorImagePath = userDetails.TryGetValue(doctor.AppUserId, out var doctorImg)
                ? $"{_baseUrl}{doctorImg.ImagePath}"
                : $"{_baseUrl}default.jpg";

            return new DoctorResponse
            {
                Id = doctor.Id.ToString(),
                AboutMe = doctor.AboutMe,
                AppointmentTypes = doctor.AppointmentTypes.Select(a => new AppointmentTypeResponse
                {
                    Id = a.Id.ToString(),
                    VisitType = a.VisitType,
                    ConsultationFee = a.ConsultationFee,
                    Duration = a.Duration,
                }).ToList(),
                FullName = doctorFullName,
                ImagePath = doctorImagePath,
                NumberOfReviews = doctor.NumberOfReviews,
                LicenseNumber = doctor.LicenseNumber,
                YearOfExperience = doctor.YearOfExperience,
                ClinicAddresses = doctor.ClinicAddresses.Select(x => new ClinicAddressResponse
                {
                    Id = x.Id,
                    City = x.City,
                    Country = x.Country,
                    Street = x.Street,
                }).ToList(),
                Specializations = doctor.Specializations.Select(x => new SpecializationResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Category = x.Category,
                }).ToList(),
                Reviews = doctor.Reviews.Select(review =>
                {
                    var reviewerFullName = userDetails.TryGetValue(review.AppUserId, out var reviewerInfo)
                        ? reviewerInfo.FullName
                        : "Unknown";

                    var reviewerImagePath = userDetails.TryGetValue(review.AppUserId, out var reviewerImg)
                        ? $"{_baseUrl}{reviewerImg.ImagePath}"
                        : $"{_baseUrl}default.jpg";

                    return new ReviewResponse
                    {
                        Id = review.Id,
                        FullName = reviewerFullName,
                        ImagePath = reviewerImagePath,
                        Comment = review.Comment,
                        Rate = review.Rate,
                    };
                }).ToList(),
                WorkingTimes = doctor.WorkingTimes.Select(x => new WorkingTimeResponse
                {
                    Id = x.Id,
                    DayOfWeek = x.DayOfWeek,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                }).ToList()
            };
        }
    
    }
    
}
