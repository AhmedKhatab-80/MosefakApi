namespace MosefakApi.Business.Services
{
    public class DoctorService : IDoctorService
    {

        private readonly IUnitOfWork<Doctor> _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public DoctorService(IUnitOfWork<Doctor> unitOfWork, IUserRepository userRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IList<DoctorResponse>> GetAllDoctors()
        {
            var doctors = await _unitOfWork.DoctorRepositoryAsync.GetDoctors();

            if (doctors == null || !doctors.Any())
            {
                return new List<DoctorResponse>(); // Return an empty list instead of throwing an exception
            }


            var appUsersIds = doctors.Select(d => d.AppUserId).ToList();

            var userDetails = await _unitOfWork.DoctorRepositoryAsync.GetUserDetailsAsync(appUsersIds);

            // Map the results
            var response = doctors.Select(d => new DoctorResponse
            {
                Id = d.Id,
                AboutMe = d.AboutMe,
                ConsultationFee = d.ConsultationFee,
                FullName = userDetails.ContainsKey(d.AppUserId) ? userDetails[d.AppUserId].FullName : "Unknown",
                ImagePath = userDetails.ContainsKey(d.AppUserId) ? userDetails[d.AppUserId].ImagePath : "",
                NumberOfReviews = d.NumberOfReviews,
                LicenseNumber = d.LicenseNumber,
                YearOfExperience = d.YearOfExperience,
                ClinicAddresses = d.ClinicAddresses.Select(x => new ClinicAddressResponse
                {
                    Id = x.Id,
                    City = x.City,
                    Country = x.Country,
                    Street = x.Street,
                })
                .ToList(),
                Specializations = d.Specializations.Select(x => new SpecializationResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Category = x.Category,
                })
                .ToList(),
                Reviews = d.Reviews.Select(x => new ReviewResponse
                {
                    Id = x.Id,
                    AppUserId = x.AppUserId,
                    Comment = x.Comment,
                    Rate = x.Rate,
                })
                .ToList(),
                WorkingTimes = d.WorkingTimes.Select(x => new WorkingTimeResponse
                {
                    Id = x.Id,
                    DayOfWeek = x.DayOfWeek,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                })
                .ToList()
            }).ToList();

            return response;

        }

        public async Task<DoctorResponse> GetDoctorById(int doctorId)
        {
            var doctor = await _unitOfWork.DoctorRepositoryAsync.GetDoctorById(doctorId);

            if (doctor is null)
                throw new ItemNotFound("Doctor is not exist");

            var userDetails = await _unitOfWork.DoctorRepositoryAsync.GetUserDetailsAsync(doctor.AppUserId);

            // Map the results
            var response = new DoctorResponse
            {
                Id = doctor.Id,
                AboutMe = doctor.AboutMe,
                ConsultationFee = doctor.ConsultationFee,
                FullName = userDetails.ContainsKey(doctor.AppUserId) ? userDetails[doctor.AppUserId].FullName : "Unknown",
                ImagePath = userDetails.ContainsKey(doctor.AppUserId) ? userDetails[doctor.AppUserId].ImagePath : "",
                NumberOfReviews = doctor.NumberOfReviews,
                LicenseNumber = doctor.LicenseNumber,
                YearOfExperience = doctor.YearOfExperience,
                ClinicAddresses = doctor.ClinicAddresses.Select(x => new ClinicAddressResponse
                {
                    Id = x.Id,
                    City = x.City,
                    Country = x.Country,
                    Street = x.Street,
                })
                .ToList(),
                Specializations = doctor.Specializations.Select(x => new SpecializationResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Category = x.Category,
                })
                .ToList(),
                Reviews = doctor.Reviews.Select(x => new ReviewResponse
                {
                    Id = x.Id,
                    AppUserId = x.AppUserId,
                    Comment = x.Comment,
                    Rate = x.Rate,
                })
                .ToList(),
                WorkingTimes = doctor.WorkingTimes.Select(x => new WorkingTimeResponse
                {
                    Id = x.Id,
                    DayOfWeek = x.DayOfWeek,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                })
                .ToList()
            };


            return response;    
        }

        public async Task<IList<DoctorDto>> TopTenDoctors()
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

            var doctor = new Doctor
            {
                AppUserId = request.AppUserId,
                AboutMe = request.AboutMe,
                ConsultationFee = request.ConsultationFee,
                LicenseNumber = request.LicenseNumber,
                YearOfExperience = request.YearOfExperience,
                NumberOfReviews = 0,
                ClinicAddresses = request.ClinicAddresses.Select(x => new ClinicAddress
                {
                    Street = x.Street,
                    City = x.City,
                    Country = x.Country,
                })
                .ToList(),
                Specializations = request.Specializations.Select(s => new Specialization
                {
                    Name = s.Name,
                    Category = s.Category,
                })
                .ToList(),
                WorkingTimes = request.WorkingTimes.Select(w => new WorkingTime
                {
                    DayOfWeek = w.DayOfWeek,
                    StartTime = w.StartTime,
                    EndTime = w.EndTime,
                })
                .ToList(),
            };

            await _unitOfWork.DoctorRepositoryAsync.AddEntityAsync(doctor);
            await _unitOfWork.CommitAsync();
        }

        public async Task CompleteDoctorProfile(int appUserIdFromClaims, CompleteDoctorProfileRequest doctor)
        {
            var user = await _userRepository.GetUserByIdAsync(appUserIdFromClaims);

            if (user is null)
                throw new ItemNotFound("this doctor is not registered!");

            var doc = new Doctor
            {
                AppUserId = appUserIdFromClaims,
                AboutMe = doctor.AboutMe,
                ConsultationFee = doctor.ConsultationFee,
                LicenseNumber = doctor.LicenseNumber,
                YearOfExperience = doctor.YearOfExperience,
                ClinicAddresses = doctor.ClinicAddresses.Select(x => new ClinicAddress
                {
                    City = x.City,
                    Country = x.Country,
                    Street = x.Street,
                })
                .ToList(),
                Specializations = doctor.Specializations.Select(s => new Specialization
                {
                    Name = s.Name,
                    Category = s.Category,
                })
                .ToList(),
                WorkingTimes = doctor.WorkingTimes.Select(w => new WorkingTime
                {
                    DayOfWeek = w.DayOfWeek,
                    StartTime = w.StartTime,
                    EndTime = w.EndTime,
                })
                .ToList(),
            };

            await _unitOfWork.DoctorRepositoryAsync.AddEntityAsync(doc);
            await _unitOfWork.CommitAsync();
        }


        public async Task<DoctorProfileResponse> GetDoctorProfile(int appUserIdFromClaims)
        {
            var profile = await _unitOfWork.DoctorRepositoryAsync.GetDoctorProfile(appUserIdFromClaims);

            if (profile is null)
                return new DoctorProfileResponse();

            return profile;
        }

        public async Task UpdateDoctorProfile(DoctorProfileUpdateRequest request, int appUserIdFromClaims)
        {
            var user = await _userRepository.GetUserByIdAsync(appUserIdFromClaims);

            if (user is null)
                throw new ItemNotFound("this doctor is not registered!");

            // Update user-related fields (shared properties like name, phone, etc.)

            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.ImagePath = request.ImagePath ?? user.ImagePath;

            var doctor = await _unitOfWork.DoctorRepositoryAsync.FirstOrDefaultASync(x=> x.AppUserId == appUserIdFromClaims);

            if (doctor is null)
                throw new ItemNotFound("this profile is not exist");

            // Update doctor-specific fields
            doctor.AboutMe = request.IntroductionBreif ?? doctor.AboutMe;
            doctor.YearOfExperience = request.YearsOfExperience;

            // Handle specializations
            if (request.specializations is not null && request.specializations.Any())
            {
                // Clear existing specializations if necessary (optional)
                doctor.Specializations.Clear();

                // Add new specializations
                foreach (var specializationRequest in request.specializations)
                {
                    // Check for existing specialization by Id to avoid duplication
                    if (!doctor.Specializations.Any(s => s.Id == specializationRequest.Id))
                    {
                        doctor.Specializations.Add(new Specialization
                        {
                            Id = specializationRequest.Id, // Assuming it already exists
                            Name = specializationRequest.Name,
                            Category = specializationRequest.Category,
                        });
                    }
                }
            }

            try
            {
                // Update user in AppIdentityDbContext
                await _userRepository.UpdateUser(user);
                await _userRepository.Save();

                // Update doctor in AppDbContext
                await _unitOfWork.DoctorRepositoryAsync.UpdateEntityAsync(doctor);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                // Log the error and handle partial updates (e.g., manual rollback or retry logic)
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
        }
    }
}
