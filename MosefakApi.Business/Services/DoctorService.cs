namespace MosefakApi.Business.Services
{
    public class DoctorService : IDoctorService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;
        private readonly IImageService _imageService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly string _baseUrl;

        public DoctorService(IUnitOfWork unitOfWork, IUserRepository userRepository, ICacheService cacheService, IImageService imageService, IConfiguration configuration, UserManager<AppUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _cacheService = cacheService;
            _imageService = imageService;
            _configuration = configuration;
            _userManager = userManager;
            _mapper = mapper;
            _baseUrl = _configuration["BaseUrl"] ?? "https://default-url.com/";
        }

        public async Task<List<DoctorResponse>> GetAllDoctors()
        {
            var doctors = await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().GetAllAsync(includes:["Specializations"]);

            if (doctors == null || !doctors.Any())
                return new List<DoctorResponse>(); // Return an empty list instead of throwing an exception

            // Extract AppUser IDs for doctors
            var appUserIds = doctors.Select(d => d.AppUserId).ToHashSet();

            // Fetch user details for doctors & reviewers in a **single batch request**
            var userDetails = await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().GetUserDetailsAsync(appUserIds);

            // Map the results
            return doctors.Select(d => MapDoctorResponse(d, userDetails)).ToList();
        }


        public async Task<DoctorDetail> GetDoctorById(int doctorId)
        {
            var doctor = await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().GetDoctorById(doctorId);

            if (doctor is null)
                throw new ItemNotFound("Doctor is not exist");

            var reviewerIds = doctor.Reviews.Select(x => x.AppUserId).ToHashSet();
            reviewerIds.Add(doctor.AppUserId);

            var userDetails = await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().GetUserDetailsAsync(reviewerIds);

            // Map the results
            return MapDoctorDetailResponse(doctor, userDetails);
        }

        public async Task<List<DoctorResponse>?> TopTenDoctors()
        {
            var query = await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().GetTopTenDoctors();

            if (query is null)
                return new List<DoctorResponse>();

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

            await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().AddEntityAsync(doctor);
            await _unitOfWork.CommitAsync();


            // ✅ Remove relevant cache entries after adding a doctor
            await RemoveCachedDoctorData(doctor.Id);
        }

        public async Task CompleteDoctorProfile(int appUserIdFromClaims, CompleteDoctorProfileRequest doctorRequest, CancellationToken cancellationToken = default)
        {
            using var transactionScope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            );

            var user = await _userRepository.GetUserByIdAsync(appUserIdFromClaims)
                ?? throw new ItemNotFound("This doctor is not registered!");

            if (await _unitOfWork.Repository<Doctor>().AnyAsync(d => d.AppUserId == appUserIdFromClaims))
                throw new BadRequest("Doctor profile has already been completed!");

            var doctor = _mapper.Map<Doctor>(doctorRequest);
            doctor.AppUserId = appUserIdFromClaims;

            doctor.Clinics = doctorRequest.Clinics?.Select(c => new Clinic
            {
                Name = c.Name,
                Street = c.Street,
                City = c.City,
                Country = c.Country,
                ApartmentOrSuite = c.ApartmentOrSuite,
                Landmark = c.Landmark,
                PhoneNumber = c.PhoneNumber,
                WorkingTimes = c.WorkingTimes?.Select(w => new WorkingTime
                {
                    Day = w.Day,
                    Periods = w.Periods.Select(p => new Period { StartTime = p.StartTime, EndTime = p.EndTime }).ToList()
                }).ToList() ?? new()
            }).ToList() ?? new();

            doctor.Specializations = doctorRequest.Specializations?
                .Select(s => new Specialization { Name = s.Name, Category = s.Category })
                .ToList() ?? new();

            doctor.AppointmentTypes = doctorRequest.AppointmentTypes?
                .Select(a => new AppointmentType { Duration = a.Duration, VisitType = a.VisitType, ConsultationFee = a.ConsultationFee })
                .ToList() ?? new();

            var universityLogos = await Task.WhenAll(doctorRequest.Educations
                .Where(e => e.UniversityLogoPath is not null)
                .Select(async e => new { e, Path = await _imageService.UploadImageOnServer("Education", e.UniversityLogoPath, false, null!, cancellationToken) }));

            doctor.Educations = doctorRequest.Educations?
                .Select(e => new Education
                {
                    Degree = e.Degree,
                    Major = e.Major,
                    UniversityName = e.UniversityName,
                    UniversityLogoPath = universityLogos.FirstOrDefault(u => u.e == e)?.Path,
                    Location = e.Location,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    CurrentlyStudying = e.CurrentlyStudying,
                    AdditionalNotes = e.AdditionalNotes
                }).ToList() ?? new();

            await _unitOfWork.Repository<Doctor>().AddEntityAsync(doctor);
            await _unitOfWork.CommitAsync();

            // assign him to doctor role after removing assigned default roles

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, DefaultRole.Doctor);

            transactionScope.Complete();

            // ✅ Remove relevant cache entries after completing the doctor profile

            await RemoveCachedDoctorData(doctor.Id);
        }


        public async Task<DoctorProfileResponse> GetDoctorProfile(int appUserIdFromClaims)
        {
            var profile = await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().GetDoctorProfile(appUserIdFromClaims);

            if (profile is null)
                return new DoctorProfileResponse();

            return profile;
        }

        public async Task UpdateDoctorProfile(DoctorProfileUpdateRequest request, int appUserIdFromClaims, CancellationToken cancellationToken = default)
        {
            // I Used TransactionScope beacuse EF Core doesn't Rollback for different Databases

            using var transactionScope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            );

            // 1) Fetch User from Identity Database
            var user = await _userRepository.GetUserByIdAsync(appUserIdFromClaims);
            if (user is null)
                throw new ItemNotFound("This doctor is not registered!");

            // 2) Fetch Doctor from Business Database
            var doctorRepo = _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>();
            var doctor = await doctorRepo.FirstOrDefaultASync(x => x.AppUserId == appUserIdFromClaims);
            if (doctor is null)
                throw new ItemNotFound("This profile does not exist!");

            bool userUpdated = false, doctorUpdated = false;

            // 3) Update User Properties (only if changed)
            if (!string.Equals(user.FirstName, request.FirstName, StringComparison.OrdinalIgnoreCase))
            {
                user.FirstName = request.FirstName!;
                userUpdated = true;
            }

            if (!string.Equals(user.LastName, request.LastName, StringComparison.OrdinalIgnoreCase))
            {
                user.LastName = request.LastName!;
                userUpdated = true;
            }

            if (!string.Equals(user.PhoneNumber, request.PhoneNumber, StringComparison.OrdinalIgnoreCase))
            {
                user.PhoneNumber = request.PhoneNumber!;
                userUpdated = true;
            }

            if (request.DateOfBirth.HasValue && user.DateOfBirth != request.DateOfBirth)
            {
                user.DateOfBirth = request.DateOfBirth.Value;
                userUpdated = true;
            }

            if (request.Gender.HasValue && user.Gender != request.Gender)
            {
                user.Gender = request.Gender.Value;
                userUpdated = true;
            }

            if (request.Address != null)
            {
                user.Address ??= new Address();
                if (!string.Equals(user.Address.Country, request.Address.State, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(user.Address.City, request.Address.City, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(user.Address.Street, request.Address.Street, StringComparison.OrdinalIgnoreCase))
                {
                    user.Address.Country = request.Address.State;
                    user.Address.City = request.Address.City;
                    user.Address.Street = request.Address.Street;
                    userUpdated = true;
                }
            }

            // 4) Update Doctor Properties (only if changed)
            if (!string.Equals(doctor.AboutMe, request.AboutMe, StringComparison.OrdinalIgnoreCase))
            {
                doctor.AboutMe = request.AboutMe!;
                doctorUpdated = true;
            }

            if (!string.Equals(doctor.LicenseNumber, request.LicenseNumber, StringComparison.OrdinalIgnoreCase))
            {
                doctor.LicenseNumber = request.LicenseNumber!;
                doctorUpdated = true;
            }

            try
            {
                // 5) Save changes only if needed
                if (userUpdated)
                {
                    await _userRepository.UpdateUser(user);
                    var rowsAffected = await _userRepository.Save();
                    if (rowsAffected <= 0)
                        throw new Exception("Failed to update user profile.");
                }

                if (doctorUpdated)
                {
                    await doctorRepo.UpdateEntityAsync(doctor);
                    await _unitOfWork.CommitAsync();
                }

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
            var doctor = await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().GetByIdAsync(doctorId);

            if (doctor is null)
                throw new ItemNotFound("Doctor is not exist");

            await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().DeleteEntityAsync(doctor);
            await _unitOfWork.CommitAsync();

            // ✅ Remove relevant cache entries after deleting a doctor
            await RemoveCachedDoctorData(doctorId);
        }


        public async Task<List<TimeSlot>> GetAvailableTimeSlots(int doctorId, int clinicId, int appointmentTypeId, DayOfWeek selectedDay)
        {
            // Step 1: Fetch Doctor's Working Periods for the Selected Clinic & Day
            var workingTime = await _unitOfWork.Repository<WorkingTime>()
                .FirstOrDefaultASync(wt => wt.ClinicId == clinicId && wt.Day == selectedDay, includes: ["Periods"]);

            if (workingTime is null || !workingTime.Periods.Any())
                return new List<TimeSlot>(); // No working periods available.

            // Step 2: Fetch Appointment Type Duration (in minutes)
            var appointmentType = await _unitOfWork.Repository<AppointmentType>()
                .FirstOrDefaultASync(at => at.Id == appointmentTypeId);

            if (appointmentType is null)
                throw new BadRequest("Invalid appointment type");

            var appointmentDuration = appointmentType.Duration; // Example: 30 minutes

            // Step 3: Fetch **All** Booked Appointments for this Doctor on Selected Day (Any Type)
            var bookedAppointments = await _unitOfWork.Repository<Appointment>()
                .GetAllAsync(a => a.DoctorId == doctorId && a.StartDate.DayOfWeek == selectedDay);

            var bookedTimes = bookedAppointments
                .Select(a => new
                {
                    StartTime = TimeOnly.FromDateTime(a.StartDate.DateTime),
                    EndTime = TimeOnly.FromDateTime(a.EndDate.DateTime)
                }).ToList();

            var availableSlots = new List<TimeSlot>();

            // Step 4: Generate Time Slots Based on **Selected Appointment Duration**
            foreach (var period in workingTime.Periods)
            {
                var slotStart = period.StartTime;
                var slotEnd = slotStart.Add(appointmentDuration);

                while (slotEnd <= period.EndTime)
                {
                    // ❌ Check if this slot **overlaps with ANY booked appointment**
                    bool isBooked = bookedTimes.Any(appt =>
                        (slotStart < appt.EndTime && slotEnd > appt.StartTime)); // Overlapping condition

                    if (!isBooked)
                    {
                        availableSlots.Add(new TimeSlot
                        {
                            StartTime = slotStart,
                            EndTime = slotEnd
                        });
                    }

                    // Move to next slot
                    slotStart = slotEnd;
                    slotEnd = slotStart.Add(appointmentDuration);
                }
            }

            return availableSlots;
        }


        public async Task<bool> UploadProfileImageAsync(int doctorId, IFormFile imageFile, CancellationToken cancellationToken = default) // [Done]
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new BadRequest("Invalid image file.");

            // 🔹 Validate Image File (Size & Format)
            ValidateImageFile(imageFile);

            // 🔹 Fetch Doctor (From Primary Database)
            var doctor = await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().AnyAsync(x=> x.AppUserId == doctorId);
            if (!doctor)
                throw new ItemNotFound("Doctor does not exist.");

            // 🔹 Fetch User Details (From Secondary Database)
            var doctorDetails = await _userRepository.GetUserByIdAsync(doctorId);
            if (doctorDetails is null)
                throw new ItemNotFound("Doctor user details not found.");

            var oldImagePath = doctorDetails.ImagePath ?? string.Empty;
            string? newImagePath = null;

            try
            {
                // 🔹 Upload New Image
                newImagePath = await _imageService.UploadImageOnServer("Doctor", imageFile, false, oldImagePath, cancellationToken);
                doctorDetails.ImagePath = newImagePath;

                // 🔹 Use Transaction to Ensure Atomicity
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                
                await _userRepository.UpdateUser(doctorDetails);
              
                var rowsAffected = await _userRepository.Save(); // ✅ Update in Secondary DB
                if (rowsAffected > 0)
                {
                    await transaction.CommitAsync();

                    // 🔹 Delete Old Image (Only If DB Commit is Successful)
                    if (!string.IsNullOrEmpty(oldImagePath))
                        await _imageService.RemoveImage($"Doctor/{oldImagePath}");

                    return true;
                }
                else
                {
                    await transaction.RollbackAsync();

                    // 🔹 Rollback Image Upload if DB Update Fails
                    if (!string.IsNullOrEmpty(newImagePath))
                        await _imageService.RemoveImage($"Doctor/{newImagePath}");

                    throw new BadRequest("Failed to update image.");
                }
            }
            catch (Exception ex)
            {
                throw new BadRequest("Failed to upload doctor profile image.", ex);
            }
        }


        public async Task<bool> UpdateWorkingTimesAsync(int doctorId, int clinicId, IEnumerable<WorkingTimeRequest> workingTimes) // [Done]
        {
            // 🔹 Fetch clinic along with WorkingTimes & Periods
            var clinic = await _unitOfWork.GetCustomRepository<ClinicRepository>().FirstOrDefaultASync(x => x.Id == clinicId && 
                                                                                        x.Doctor.AppUserId == doctorId, ["WorkingTimes" , "WorkingTimes.Periods","Doctor"]);

            if (clinic is null)
                throw new ItemNotFound("Clinic does not exist or you do not have permission to modify it.");

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var existingWorkingTimes = clinic.WorkingTimes.ToList();

                foreach (var request in workingTimes)
                {
                    var existingWorkingTime = existingWorkingTimes.FirstOrDefault(x => x.Day == request.Day);

                    if (existingWorkingTime == null)
                    {
                        // 🔹 If No Existing WorkingTime → Add New
                        var newWorkingTime = new WorkingTime
                        {
                            ClinicId = clinicId,
                            Day = request.Day,
                            Periods = request.Periods.Select(p => new Period
                            {
                                StartTime = p.StartTime,
                                EndTime = p.EndTime
                            }).ToHashSet()
                        };

                        clinic.WorkingTimes.Add(newWorkingTime);
                    }
                    else
                    {
                        // 🔹 If WorkingTime Exists → Update Periods
                        existingWorkingTime.Periods.Clear(); // Remove old periods
                        foreach (var periodRequest in request.Periods)
                        {
                            existingWorkingTime.Periods.Add(new Period
                            {
                                StartTime = periodRequest.StartTime,
                                EndTime = periodRequest.EndTime
                            });
                        }
                    }
                }

                var rowsAffected = await _unitOfWork.CommitAsync();
                await transaction.CommitAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to update working times.", ex);
            }
        }

        public async Task<List<DoctorResponse>> SearchDoctorsAsync(DoctorSearchFilter filter)
        {
            if (string.IsNullOrWhiteSpace(filter.Name))
                return new List<DoctorResponse>();

            var normalizedName = filter.Name.Trim().ToLower();

            // 🔹 Fetch users matching the search filter
            var users = await _userRepository.GetAllUsersAsync(x =>
                (x.FirstName + " " + x.LastName).ToLower().Contains(normalizedName));

            if (!users.Any())
                return new List<DoctorResponse>();

            // 🔹 Create a dictionary for fast lookup
            var userDictionary = users.ToDictionary(
                u => u.Id,
                u => new
                {
                    FullName = $"{u.FirstName} {u.LastName}",
                    ImagePath = _baseUrl + (string.IsNullOrWhiteSpace(u.ImagePath) ? "default.jpg" : u.ImagePath)
                });

            var userIds = users.Select(x => x.Id).ToList();

            // 🔹 Retrieve doctor details
            var doctors = await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>()
                                           .GetAllAsync(d => userIds.Contains(d.AppUserId), ["Specializations"]);

            // 🔹 Map the final result
            return doctors.Select(d => new DoctorResponse
            {
                Id = d.Id.ToString(),
                FullName = userDictionary[d.AppUserId].FullName, // ✅ Directly use existing data
                ImagePath = userDictionary[d.AppUserId].ImagePath, // ✅ Avoid extra query
                TotalYearsOfExperience = d.TotalYearsOfExperience,
                Specializations = d.Specializations.Select(x => new SpecializationResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Category = x.Category
                }).ToList(),
                NumberOfReviews = d.NumberOfReviews,
            }).ToList();
        }


        public async Task<IEnumerable<AppointmentDto>?> GetUpcomingAppointmentsAsync(int doctorId)
        {
            return await GetAppointmentsAsync(doctorId, isUpcoming: true);
        }

        public async Task<IEnumerable<AppointmentDto>?> GetPastAppointmentsAsync(int doctorId)
        {
            return await GetAppointmentsAsync(doctorId, isUpcoming: false);
        }

        public async Task<long> GetTotalAppointmentsAsync(int doctorId)
        {
            // 🔹 Step 1: Ensure Doctor Exists
            var doctorExists = await _unitOfWork.Repository<Doctor>().AnyAsync(x => x.AppUserId == doctorId);
            if (!doctorExists)
                throw new ItemNotFound("Doctor does not exist.");

            var count = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>()
                .GetCountWithConditionAsync(a=> a.DoctorId == doctorId);

            return count;
        }

        public async Task<bool> AddSpecializationAsync(int doctorId, SpecializationRequest request)
        {
            // 🔹 Fetch Doctor 
            var doctor = await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().FirstOrDefaultASync(x=> x.AppUserId == doctorId, ["Specializations"]);
            if (doctor is null)
                throw new ItemNotFound("Doctor does not exist.");

            var isSpecializationExist = doctor.Specializations.Any(x => x.DoctorId == doctor.Id && x.Name == request.Name && x.Category == request.Category);

            if (isSpecializationExist)
                throw new BadRequest("Specialization already exists for this doctor.");

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                doctor.Specializations.Add(new Specialization
                {
                    DoctorId = doctor.Id,
                    Name = request.Name,
                    Category = request.Category,
                });

                await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().UpdateEntityAsync(doctor);

                var rowsAffected = await _unitOfWork.CommitAsync();

                if (rowsAffected <= 0)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequest("Failed to add specialization.");
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to add specialization.", ex);
            }
        }

        public async Task<bool> RemoveSpecializationAsync(int doctorId, int specializationId)
        {
            var specialization = await _unitOfWork.Repository<Specialization>().FirstOrDefaultASync(x => x.Id == specializationId &&
                                                                                                          x.Doctor.AppUserId == doctorId, ["Doctor"]);

            if (specialization == null)
                throw new ItemNotFound("Specialization is not exist");

            var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Repository<Specialization>().DeleteEntityAsync(specialization);
                var rowsAffected = await _unitOfWork.CommitAsync();

                if (rowsAffected <= 0)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequest("Failed to remove specialization.");
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to remove specialization.", ex);
            }
        }

        public async Task<bool> EditSpecializationAsync(int doctorId, int specializationId, SpecializationRequest request)
        {
            var specialization = await _unitOfWork.Repository<Specialization>().FirstOrDefaultASync(x => x.Id == specializationId &&
                                                                                                          x.Doctor.AppUserId == doctorId, ["Doctor"]);

            if (specialization == null)
                throw new ItemNotFound("Specialization is not exist");

            var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                specialization.Name = request.Name;
                specialization.Category = request.Category;

                await _unitOfWork.Repository<Specialization>().UpdateEntityAsync(specialization);
                var rowsAffected = await _unitOfWork.CommitAsync();

                if (rowsAffected <= 0)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequest("Failed to Edit specialization.");
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to Edit specialization.", ex);
            }
        }

        public async Task<bool> AddExperienceAsync(int doctorId, ExperienceRequest request, CancellationToken cancellationToken = default)
        {
            // 🔹 Fetch Doctor 
            var doctor = await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().FirstOrDefaultASync(x => x.AppUserId == doctorId, ["Experiences"]);
            if (doctor is null)
                throw new ItemNotFound("Doctor does not exist.");

            // 🔹 Step 2: Check for Duplicate Experience
            var experienceExists = doctor.Experiences.Any(e =>
                e.DoctorId == doctor.Id && e.Title == request.Title && e.StartDate == request.StartDate && e.EndDate == request.EndDate);

            if (experienceExists)
                throw new BadRequest("Experience already exists for this doctor.");

            // 🔹 Step 3: Validate & Upload Hospital Logo (If Exists)
            string? logoPath = null;
            if (request.HospitalLogo is not null)
            {
                ValidateImageFile(request.HospitalLogo); // 🔥 Validate File Before Upload
                logoPath = await _imageService.UploadImageOnServer("Hospital", request.HospitalLogo, false, null!, cancellationToken);
            }
           
            doctor.Experiences.Add(new Experience
            {
                DoctorId = doctor.Id,
                Title = request.Title,
                EmploymentType = request.EmploymentType,
                CurrentlyWorkingHere = request.CurrentlyWorkingHere,
                HospitalName = request.HospitalName,
                EndDate = request.EndDate,
                StartDate = request.StartDate,
                HospitalLogo = logoPath,
                Location = request.Location,
                JobDescription = request.JobDescription,
            });

            // 🔹 Step 5: Save to Database
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().UpdateEntityAsync(doctor);
                var rowsAffected = await _unitOfWork.CommitAsync();

                if (rowsAffected <= 0)
                {
                    throw new BadRequest("Failed to add experience.");
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // 🔥 Remove uploaded image if DB transaction fails
                if (!string.IsNullOrEmpty(logoPath))
                {
                    await _imageService.RemoveImage($"Hospital/{logoPath}");
                }

                throw new Exception("Failed to add experience.", ex);
            }
        }

        public async Task<bool> EditExperienceAsync(int doctorId, int experienceId, ExperienceRequest request, CancellationToken cancellationToken = default)
        {
            // 🔹 Step 1: Ensure Experience Exists for the Given Doctor
            var experience = await _unitOfWork.Repository<Experience>().FirstOrDefaultASync(x => x.Id == experienceId && x.Doctor.AppUserId == doctorId, ["Doctor"]);

            if (experience == null)
                throw new ItemNotFound("Experience does not exist.");

            // 🔹 Step 2: Check for Duplicate Experience Data
            var isDuplicate = await _unitOfWork.Repository<Experience>().AnyAsync(e =>
                e.Id != experienceId && e.DoctorId == experience.DoctorId &&
                e.Title == request.Title && e.StartDate == request.StartDate && e.EndDate == request.EndDate);

            if (isDuplicate)
                throw new BadRequest("An experience with the same details already exists for this doctor.");

            // 🔹 Step 3: Validate & Upload Hospital Logo (If Exists)
            string? newLogoPath = null;
            string oldLogoPath = experience.HospitalLogo ?? string.Empty;

            try
            {
                if (request.HospitalLogo is not null)
                {
                    ValidateImageFile(request.HospitalLogo); // 🔥 Validate Before Upload
                    newLogoPath = await _imageService.UploadImageOnServer("Hospital", request.HospitalLogo, false, null!, cancellationToken);
                }

                // 🔹 Step 4: Update Experience Entity
                experience.Title = request.Title;
                experience.EmploymentType = request.EmploymentType;
                experience.CurrentlyWorkingHere = request.CurrentlyWorkingHere;
                experience.HospitalName = request.HospitalName;
                experience.EndDate = request.EndDate;
                experience.StartDate = request.StartDate;
                experience.HospitalLogo = newLogoPath ?? oldLogoPath; // Keep old logo if no new one is uploaded
                experience.Location = request.Location;
                experience.JobDescription = request.JobDescription;

                using var transaction = await _unitOfWork.BeginTransactionAsync();

                await _unitOfWork.Repository<Experience>().UpdateEntityAsync(experience);
                var rowsAffected = await _unitOfWork.CommitAsync();

                if (rowsAffected <= 0)
                {
                    await transaction.RollbackAsync();

                    // 🔥 Remove uploaded new image if DB commit fails
                    if (!string.IsNullOrEmpty(newLogoPath))
                        await _imageService.RemoveImage($"Hospital/{newLogoPath}");

                    throw new BadRequest("Failed to edit experience.");
                }

                await transaction.CommitAsync();

                // 🔹 Step 5: Remove Old Image Only After Successful Update
                if (!string.IsNullOrEmpty(newLogoPath) && !string.IsNullOrEmpty(oldLogoPath) && newLogoPath != oldLogoPath)
                    await _imageService.RemoveImage($"Hospital/{oldLogoPath}");

                return true;
            }
            catch (Exception ex)
            {
                // 🔥 Ensure Transaction is Rolled Back on Exception
                await _unitOfWork.RollbackTransactionAsync();

                // 🔥 Remove new image if an error occurred
                if (!string.IsNullOrEmpty(newLogoPath))
                    await _imageService.RemoveImage($"Hospital/{newLogoPath}");

                throw new Exception("Failed to edit experience.", ex);
            }
        }

        public async Task<bool> RemoveExperienceAsync(int doctorId, int experienceId)
        {
            var experience = await _unitOfWork.Repository<Experience>().FirstOrDefaultASync(x => x.Id == experienceId && x.Doctor.AppUserId == doctorId, ["Doctor"]);

            if (experience == null)
                throw new ItemNotFound("experience is not exist");

            var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Repository<Experience>().DeleteEntityAsync(experience);
                var rowsAffected = await _unitOfWork.CommitAsync();

                if (rowsAffected <= 0)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequest("Failed to remove Experience.");
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to remove Experience.", ex);
            }
        }

        public async Task<bool> AddAwardAsync(int doctorId, AwardRequest request, CancellationToken cancellationToken = default)
        {
            // 🔹 Step 1: Ensure Doctor Exists
            var doctor = await _unitOfWork.Repository<Doctor>().FirstOrDefaultASync(x => x.AppUserId == doctorId);
            if (doctor is null)
                throw new ItemNotFound("Doctor does not exist.");

            // 🔹 Step 2: Normalize & Validate Award Data
            var normalizedTitle = request.Title?.Trim().ToLower();
            var normalizedDescription = request.Description?.Trim().ToLower();
            var normalizedOrganization = request.Organization?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(normalizedTitle) || string.IsNullOrWhiteSpace(normalizedOrganization))
                throw new BadRequest("Title and Organization fields are required.");

            // 🔹 Step 3: Check for Duplicate Award
            var isDuplicated = await _unitOfWork.Repository<Award>().AnyAsync(a =>
                a.DoctorId == doctor.Id && 
                a.Title.ToLower() == normalizedTitle &&
                a.Organization.ToLower() == normalizedOrganization &&
                (a.Description == null || a.Description.ToLower() == normalizedDescription));

            if (isDuplicated)
                throw new ItemAlreadyExist("This award already exists.");

            // 🔹 Step 4: Insert New Award
            var award = new Award
            {
                DoctorId = doctor.Id,
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                Organization = request.Organization.Trim(),
            };

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Repository<Award>().AddEntityAsync(award);
                var rowsAffected = await _unitOfWork.CommitAsync();

                if (rowsAffected <= 0)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequest("Failed to add award.");
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to add award.", ex);
            }
        }


        public async Task<bool> EditAwardAsync(int doctorId, int awardId, AwardRequest request)
        {
            // 🔹 Step 1: Normalize & Validate Award Data
            var (normalizedTitle, normalizedDescription, normalizedOrganization) = NormalizeAwardRequest(request);

            if (string.IsNullOrWhiteSpace(normalizedTitle) || string.IsNullOrWhiteSpace(normalizedOrganization))
                throw new BadRequest("Title and Organization fields are required.");

            // 🔹 Step 2: Fetch Award with Doctor Validation
            var award = await _unitOfWork.Repository<Award>().FirstOrDefaultASync(a => a.Id == awardId && a.Doctor.AppUserId == doctorId, ["Doctor"]);
            if (award == null)
                throw new ItemNotFound("Award does not exist or does not belong to this doctor.");

            // 🔹 Step 3: Check for Duplicate Award
            var isDuplicate = await _unitOfWork.Repository<Award>().AnyAsync(a =>
                a.Id != awardId &&
                a.DoctorId == award.DoctorId &&
                a.Title.ToLower() == normalizedTitle &&
                a.Organization.ToLower() == normalizedOrganization &&
                (a.Description == null || a.Description.ToLower() == normalizedDescription));

            if (isDuplicate)
                throw new ItemAlreadyExist("An award with the same details already exists for this doctor.");

            // 🔹 Step 4: Update Award Details
            award.Title = request.Title.Trim();
            award.Description = request.Description?.Trim();
            award.Organization = request.Organization.Trim();

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Repository<Award>().UpdateEntityAsync(award);
                var rowsAffected = await _unitOfWork.CommitAsync();

                if (rowsAffected <= 0)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequest("Failed to update award.");
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to update award.", ex);
            }
        }


        public async Task<bool> RemoveAwardAsync(int doctorId, int awardId)
        {
            // 🔹 Step 1: Fetch Award with Doctor Validation
            var award = await _unitOfWork.Repository<Award>().FirstOrDefaultASync(a => a.Id == awardId && a.Doctor.AppUserId == doctorId, ["Doctor"]);

            if (award == null)
                throw new ItemNotFound("Award does not exist or does not belong to this doctor.");

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Repository<Award>().DeleteEntityAsync(award);
                var rowsAffected = await _unitOfWork.CommitAsync();

                if (rowsAffected <= 0)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequest("Failed to delete award.");
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to delete award.", ex);
            }
        }


        public async Task<bool> AddEducationAsync(int doctorId, EducationRequest request, CancellationToken cancellationToken = default)
        {
            // 🔹 Step 1: Ensure Doctor Exists
            var doctor = await _unitOfWork.Repository<Doctor>().FirstOrDefaultASync(x => x.AppUserId == doctorId);
            if (doctor is null)
                throw new ItemNotFound("Doctor does not exist.");

            // 🔹 Step 2: Check for Duplicate Education Entry
            var isDuplicated = await _unitOfWork.Repository<Education>().AnyAsync(x =>
                x.DoctorId == doctor.Id &&
                x.UniversityName == request.UniversityName &&
                x.Degree == request.Degree &&
                x.StartDate == request.StartDate &&
                x.Major == request.Major &&
                x.Location == request.Location);

            if (isDuplicated)
                throw new ItemAlreadyExist("This education record already exists.");

            string? newImagePath = null;

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 🔹 Step 3: Upload University Logo (if provided)
                if (request.UniversityLogoPath is not null)
                    newImagePath = await _imageService.UploadImageOnServer("Education", request.UniversityLogoPath, false, null!, cancellationToken);

                // 🔹 Step 4: Create & Add New Education Entry
                var education = new Education
                {
                    DoctorId = doctor.Id,
                    UniversityName = request.UniversityName.Trim(),
                    Degree = request.Degree.Trim(),
                    Major = request.Major?.Trim()!,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    CurrentlyStudying = request.CurrentlyStudying,
                    Location = request.Location?.Trim()!,
                    AdditionalNotes = request.AdditionalNotes?.Trim(),
                    UniversityLogoPath = newImagePath
                };

                await _unitOfWork.Repository<Education>().AddEntityAsync(education);

                var rowsAffected = await _unitOfWork.CommitAsync();
                if (rowsAffected <= 0)
                    throw new BadRequest("Failed to add education.");

                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                // 🔹 Step 5: Remove Uploaded Image If Transaction Fails
                if (!string.IsNullOrEmpty(newImagePath))
                {
                    await _imageService.RemoveImage($"Education/{newImagePath}");
                }

                throw new Exception("Failed to add education.", ex);
            }
        }


        public async Task<bool> EditEducationAsync(int doctorId, int educationId, EducationRequest request, CancellationToken cancellationToken = default)
        {
            // 🔹 Step 1: Ensure Education Exists & Belongs to Doctor
            var education = await _unitOfWork.Repository<Education>().FirstOrDefaultASync(x => x.Id == educationId && x.Doctor.AppUserId == doctorId, ["Doctor"]);
            if (education is null)
                throw new ItemNotFound("Education record does not exist or you have no permission.");

            string oldImagePath = education.UniversityLogoPath ?? string.Empty;
            string? newImagePath = null;

            // 🔹 Step 2: Upload New Image (if provided)
            if (request.UniversityLogoPath != null)
                newImagePath = await _imageService.UploadImageOnServer("Education", request.UniversityLogoPath, false, oldImagePath, cancellationToken);

            // 🔹 Step 3: Update Education Entity
            education.StartDate = request.StartDate;
            education.EndDate = request.EndDate;
            education.Location = request.Location?.Trim()!;
            education.AdditionalNotes = request.AdditionalNotes?.Trim();
            education.CurrentlyStudying = request.CurrentlyStudying;
            education.Degree = request.Degree.Trim();
            education.Major = request.Major?.Trim()!;
            education.UniversityName = request.UniversityName.Trim();
            education.UniversityLogoPath = newImagePath ?? oldImagePath;

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Repository<Education>().UpdateEntityAsync(education);
                var rowsAffected = await _unitOfWork.CommitAsync();

                if (rowsAffected <= 0)
                    throw new BadRequest("Failed to edit Education.");

                await transaction.CommitAsync(cancellationToken);

                // 🔹 Step 4: Remove Old Image Only After Successful Update
                if (!string.IsNullOrEmpty(newImagePath) && !string.IsNullOrEmpty(oldImagePath) && newImagePath != oldImagePath)
                {
                    await _imageService.RemoveImage($"Education/{oldImagePath}");
                }

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                // 🔥 Remove newly uploaded image if the transaction fails
                if (!string.IsNullOrEmpty(newImagePath))
                {
                    await _imageService.RemoveImage($"Education/{newImagePath}");
                }

                throw new Exception("Failed to edit Education.", ex);
            }
        }


        public async Task<bool> RemoveEducationAsync(int doctorId, int educationId)
        {
            var education = await _unitOfWork.Repository<Education>().FirstOrDefaultASync(x => x.Id == educationId && x.Doctor.AppUserId == doctorId, ["Doctor"]);
            
            if (education is null)
                throw new ItemNotFound("Doctor does not exist or you have no permission.");

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Repository<Education>().DeleteEntityAsync(education);
                var rowsAffected = await _unitOfWork.CommitAsync();

                if (rowsAffected <= 0)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequest("Failed to delete education.");
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to delete education.", ex);
            }
        }

        public async Task<bool> AddClinicAsync(int doctorId, ClinicRequest request, CancellationToken cancellationToken = default)
        {
            // 🔹 Step 1: Ensure Doctor Exists
            var doctor = await _unitOfWork.Repository<Doctor>().FirstOrDefaultASync(x => x.AppUserId == doctorId);
            if (doctor is null)
                throw new ItemNotFound("Doctor does not exist.");

            // 🔹 Step 2: Check Duplication
            var isDuplicated = await _unitOfWork.Repository<Clinic>().AnyAsync(
                x => x.Street.Trim().ToLower() == request.Street.Trim().ToLower() &&
                     x.City.Trim().ToLower() == request.City.Trim().ToLower() &&
                     x.Country.Trim().ToLower() == request.Country.Trim().ToLower() &&
                     x.DoctorId == doctor.Id &&
                     x.Name.Trim().ToLower() == request.Name.Trim().ToLower() &&
                     x.PhoneNumber.Trim() == request.PhoneNumber.Trim());

            if (isDuplicated)
                throw new ItemAlreadyExist("Clinic already exists.");

            string? clinicNewPath = null;
            string? clinicNewLogoPath = null;

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 🔹 Step 3: Upload Images (If Provided)
                if (request.ClinicImage != null)
                {
                    clinicNewPath = await _imageService.UploadImageOnServer("Clinic", request.ClinicImage, false, null!, cancellationToken);
                }

                if (request.LogoPath != null)
                {
                    clinicNewLogoPath = await _imageService.UploadImageOnServer("Clinic", request.LogoPath, false, null!, cancellationToken);
                }

                // 🔹 Step 4: Create Clinic Entity
                var clinic = new Clinic
                {
                    DoctorId = doctor.Id,
                    Name = request.Name.Trim(),
                    Street = request.Street.Trim(),
                    City = request.City.Trim(),
                    Country = request.Country.Trim(),
                    ApartmentOrSuite = request.ApartmentOrSuite?.Trim()!,
                    Landmark = request.Landmark?.Trim()!,
                    PhoneNumber = request.PhoneNumber.Trim(),
                    LogoPath = clinicNewLogoPath,
                    ClinicImage = clinicNewPath, 
                    WorkingTimes = request.WorkingTimes
                        .Select(w => new WorkingTime
                        {
                            Day = w.Day,
                            Periods = w.Periods
                                .Select(p => new Period
                                {
                                    StartTime = p.StartTime,
                                    EndTime = p.EndTime,
                                })
                                .ToList()
                        })
                        .ToList(),
                };

                await _unitOfWork.Repository<Clinic>().AddEntityAsync(clinic);
                var rowsAffected = await _unitOfWork.CommitAsync();

                if (rowsAffected <= 0)
                    throw new BadRequest("Failed to add clinic.");

                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync(cancellationToken);

                // 🔹 Step 5: Remove Uploaded Image If Transaction Fails
                if (!string.IsNullOrEmpty(clinicNewLogoPath))
                    await _imageService.RemoveImage($"Clinic/{clinicNewLogoPath}");

                if (!string.IsNullOrEmpty(clinicNewPath))
                    await _imageService.RemoveImage($"Clinic/{clinicNewPath}");

                throw new Exception("Database error occurred while adding clinic.", dbEx);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                if (!string.IsNullOrEmpty(clinicNewLogoPath))
                    await _imageService.RemoveImage($"Clinic/{clinicNewLogoPath}");

                if (!string.IsNullOrEmpty(clinicNewPath))
                    await _imageService.RemoveImage($"Clinic/{clinicNewPath}");

                throw new Exception("Failed to add clinic.", ex);
            }
        }


        public async Task<bool> EditClinicAsync(int doctorId, int clinicId, ClinicRequest request, CancellationToken cancellationToken = default)
        {
            // 🔹 Step 1: Retrieve Clinic with Related Entities
            var clinic = await _unitOfWork.Repository<Clinic>().FirstOrDefaultASync(
                x => x.Id == clinicId && x.Doctor.AppUserId == doctorId,
                includes: ["WorkingTimes", "WorkingTimes.Periods", "Doctor"]
            );

            if (clinic is null)
                throw new ItemNotFound("Clinic does not exist or you have no permission.");

            // 🔹 Step 2: Normalize Input & Check for Duplicates
            var normalizedStreet = request.Street.Trim().ToLower();
            var normalizedCity = request.City.Trim().ToLower();
            var normalizedCountry = request.Country.Trim().ToLower();
            var normalizedName = request.Name.Trim().ToLower();
            var normalizedPhone = request.PhoneNumber.Trim();

            var isDuplicated = await _unitOfWork.Repository<Clinic>().AnyAsync(
                x => x.Id != clinicId && // Exclude the current clinic
                     x.Street.Trim().ToLower() == normalizedStreet &&
                     x.City.Trim().ToLower() == normalizedCity &&
                     x.Country.Trim().ToLower() == normalizedCountry &&
                     x.DoctorId == clinic.DoctorId &&
                     x.Name.Trim().ToLower() == normalizedName &&
                     x.PhoneNumber.Trim() == normalizedPhone
            );

            if (isDuplicated)
                throw new ItemAlreadyExist("Clinic already exists.");

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            string? clinicNewPath = null;
            string? clinicNewLogoPath = null;

            try
            {
               
                // 🔹 Step 3: Upload Images (Only if a New Image is Provided)
                if (request.ClinicImage != null)
                {
                    clinicNewPath = await _imageService.UploadImageOnServer("Clinic", request.ClinicImage, false, clinic.ClinicImage!, cancellationToken);
                }

                if (request.LogoPath != null)
                {
                    clinicNewLogoPath = await _imageService.UploadImageOnServer("Clinic", request.LogoPath, false, clinic.LogoPath!, cancellationToken);
                }

                // 🔹 Step 4: Update Clinic Fields
                clinic.Name = request.Name;
                clinic.Street = request.Street;
                clinic.City = request.City;
                clinic.Country = request.Country;
                clinic.Landmark = request.Landmark;
                clinic.PhoneNumber = request.PhoneNumber;
                clinic.ApartmentOrSuite = request.ApartmentOrSuite;
                clinic.ClinicImage = clinicNewPath ?? clinic.ClinicImage;
                clinic.LogoPath = clinicNewLogoPath ?? clinic.LogoPath;

                // 🔹 Step 5: Update Working Times (If Provided)
                if (request.WorkingTimes.Any())
                {
                    clinic.WorkingTimes.Clear();

                    foreach (var w in request.WorkingTimes)
                    {
                        clinic.WorkingTimes.Add(new WorkingTime()
                        {
                            Day = w.Day,
                            Periods = w.Periods.Select(p => new Period
                            {
                                StartTime = p.StartTime,
                                EndTime = p.EndTime,
                            }).ToList()
                        });
                    }
                }

                await _unitOfWork.Repository<Clinic>().UpdateEntityAsync(clinic);
                var rowsAffected = await _unitOfWork.CommitAsync();

                if (rowsAffected <= 0)
                    throw new BadRequest("Failed to edit clinic.");

                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                // 🔹 Step 6: Remove Uploaded Images If Transaction Fails
                if (!string.IsNullOrEmpty(clinicNewLogoPath))
                    await _imageService.RemoveImage($"Clinic/{clinicNewLogoPath}");

                if (!string.IsNullOrEmpty(clinicNewPath))
                    await _imageService.RemoveImage($"Clinic/{clinicNewPath}");

                throw new Exception("Failed to edit clinic.", ex);
            }
        }


        public async Task<bool> RemoveClinicAsync(int doctorId, int clinicId)
        {
            // 🔹 Step 1: Retrieve the Clinic
            var clinic = await _unitOfWork.Repository<Clinic>().FirstOrDefaultASync(x => x.Id == clinicId && x.Doctor.Id == doctorId, ["Doctor"]);

            if (clinic is null)
                throw new ItemNotFound("Clinic does not exist or you have no permission.");

            var clinicImagePath = clinic.ClinicImage;
            var clinicLogoPath = clinic.LogoPath;

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 🔹 Step 2: Remove the Clinic
                await _unitOfWork.Repository<Clinic>().DeleteEntityAsync(clinic);
                var rowsAffected = await _unitOfWork.CommitAsync();

                if (rowsAffected <= 0)
                    throw new BadRequest("Failed to remove clinic.");

                await transaction.CommitAsync();

                // 🔹 Step 3: Remove Images After Successful Deletion
                if (!string.IsNullOrEmpty(clinicImagePath))
                    await _imageService.RemoveImage($"Clinic/{clinicImagePath}");

                if (!string.IsNullOrEmpty(clinicLogoPath))
                    await _imageService.RemoveImage($"Clinic/{clinicLogoPath}");

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to remove clinic.", ex);
            }
        }


        public async Task<List<ClinicResponse>> GetDoctorClinicsAsync(int doctorId)
        {
            var clinics = await _unitOfWork.Repository<Clinic>()
                                           .GetAllAsync(c => c.Doctor.AppUserId == doctorId, ["WorkingTimes", "WorkingTimes.Periods","Doctor"]);

            return (List<ClinicResponse>)(clinics?.Any() == true ? _mapper.Map<List<ClinicResponse>>(clinics) : Enumerable.Empty<ClinicResponse>());
        }

        public async Task<List<ReviewResponse>?> GetDoctorReviewsAsync(int doctorId)
        {
            var reviews = await _unitOfWork.Repository<Review>().GetAllAsync(r => r.DoctorId == doctorId);

            return (List<ReviewResponse>)(reviews?.Any() == true ? _mapper.Map<List<ReviewResponse>>(reviews) : Enumerable.Empty<ReviewResponse>());
        }

        public async Task<double> GetAverageRatingAsync(int doctorId)
        {
            return await _unitOfWork.Repository<Review>().GetAverage(r => r.Rate, r => r.DoctorId == doctorId);
        }


        public async Task<long> GetTotalPatientsServedAsync(int doctorId)
        {
            // 🔹 Step 1: Ensure Doctor Exists
            var doctor = await _unitOfWork.Repository<Doctor>().FirstOrDefaultASync(x => x.AppUserId == doctorId);
            if (doctor is null)
                throw new ItemNotFound("Doctor does not exist.");

            // 🔹 Step 2: Query Efficiently Using Indexed Columns
            return await _unitOfWork.Repository<Appointment>().GetCountWithConditionAsync(
                x => x.DoctorId == doctor.Id && x.AppointmentStatus == AppointmentStatus.Completed);
        }

        public Task<DoctorEarningsResponse> GetEarningsReportAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        private void ValidateImageFile(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var maxFileSize = 2 * 1024 * 1024; // 2MB

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
                throw new BadRequest($"Invalid file type. Allowed types: {string.Join(", ", allowedExtensions)}");

            if (file.Length > maxFileSize)
                throw new BadRequest("File size exceeds the 2MB limit.");
        }

        private (string Title, string Description, string Organization) NormalizeAwardRequest(AwardRequest request)
        {
            return (
                request.Title?.Trim().ToLower() ?? string.Empty,
                request.Description?.Trim().ToLower() ?? string.Empty,
                request.Organization?.Trim().ToLower() ?? string.Empty
            );
        }

        // 🔹 Private helper method to avoid duplication
        private async Task<IEnumerable<AppointmentDto>> GetAppointmentsAsync(int doctorId, bool isUpcoming)
        {
            var now = DateTimeOffset.UtcNow;

            var appointments = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().GetAllAsync(
                x => x.Doctor.Id == doctorId && (isUpcoming ? x.StartDate > now : x.StartDate < now),
                includes: ["AppointmentType", "Doctor"]
            );

            // 🔹 Always return a non-null collection
            return appointments?.Select(a => _mapper.Map<AppointmentDto>(a)) ?? Enumerable.Empty<AppointmentDto>();
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

                FullName = doctorFullName,
                ImagePath = doctorImagePath,
                NumberOfReviews = doctor.NumberOfReviews,
                TotalYearsOfExperience = doctor.TotalYearsOfExperience,
                Specializations = doctor.Specializations.Select(x => new SpecializationResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Category = x.Category,
                }).ToList(),
            };

        }

        private DoctorDetail MapDoctorDetailResponse(
           Doctor doctor,
           Dictionary<int, (string FullName, string ImagePath)> userDetails)
        {
            var doctorFullName = userDetails.TryGetValue(doctor.AppUserId, out var doctorInfo)
                ? doctorInfo.FullName
                : "Unknown";

            var doctorImagePath = userDetails.TryGetValue(doctor.AppUserId, out var doctorImg)
                ? $"{_baseUrl}{doctorImg.ImagePath}"
                : $"{_baseUrl}default.jpg";

            return new DoctorDetail
            {
                Id = doctor.Id.ToString(),
                FullName = doctorFullName,
                ImagePath = doctorImagePath,
                NumberOfReviews = doctor.NumberOfReviews,
                TotalYearsOfExperience = doctor.TotalYearsOfExperience,
                AboutMe = doctor.AboutMe,
                AppointmentTypes = doctor.AppointmentTypes.Select(x => new AppointmentTypeResponse
                {
                    ConsultationFee = x.ConsultationFee,
                    Duration = x.Duration,
                    Id = x.Id.ToString(),
                    VisitType = x.VisitType,
                })
                .ToList(),
                Awards = doctor.Awards.Select(a => new AwardResponse
                {
                    Id = a.Id,
                    DateReceived = a.DateReceived,
                    Description = a.Description,
                    DoctorId = a.DoctorId,
                    Organization = a.Organization,
                    Title = a.Title,
                })
                .ToList(),
                Educations = doctor.Educations.Select(e => new EducationResponse
                {
                    DoctorId = e.DoctorId,
                    AdditionalNotes = e.AdditionalNotes,
                    CurrentlyStudying = e.CurrentlyStudying,
                    Degree = e.Degree,
                    EndDate = e.EndDate,
                    Id = e.Id,
                    Location = e.Location,
                    Major = e.Major,
                    StartDate = e.StartDate,
                    UniversityLogoPath = $"{_baseUrl}{e.UniversityLogoPath}",
                    UniversityName = e.UniversityName
                })
                .ToList(),
                Clinics = doctor.Clinics.Select(x => new ClinicResponse
                {
                    Id = x.Id.ToString(),
                    City = x.City,
                    Country = x.Country,
                    Street = x.Street,
                    ApartmentOrSuite = x.ApartmentOrSuite,
                    ClinicImage = x.ClinicImage,
                    Landmark = x.Landmark,
                    DoctorId = x.DoctorId,
                    LogoPath = x.LogoPath,
                    Name = x.Name,
                    PhoneNumber = x.PhoneNumber,
                    WorkingTimes = x.WorkingTimes.Select(w => new WorkingTimeResponse
                    {
                        ClinicId = w.Id,
                        Day = w.Day,
                        Id = w.Id,
                        IsAvailable = w.IsAvailable,
                        Periods = w.Periods.Select(p => new PeriodResponse
                        {
                            Id = p.Id,
                            IsAvailable = p.IsAvailable,
                            EndTime = p.EndTime,
                            StartTime = p.StartTime,
                            WorkingTimeId = p.WorkingTimeId
                        }).ToList(),
                    }).ToList()
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
                        Id = review.Id.ToString(),
                        FullName = reviewerFullName,
                        ImagePath = reviewerImagePath,
                        Comment = review.Comment,
                        Rate = review.Rate,
                    };
                }).ToList(),
            };
        }
    }

}
