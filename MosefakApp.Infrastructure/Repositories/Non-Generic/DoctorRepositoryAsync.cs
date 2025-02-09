namespace MosefakApp.Infrastructure.Repositories.Non_Generic
{
    public class DoctorRepositoryAsync : GenericRepositoryAsync<Doctor>, IDoctorRepositoryAsync
    {
        private readonly AppDbContext _appDbContext;
        private readonly AppIdentityDbContext _identityDbContext;
        private readonly IConfiguration _config;
        private readonly string _baseUrl;
        public DoctorRepositoryAsync(AppDbContext context, AppIdentityDbContext identityDbContext, IConfiguration config) : base(context)
        {
            _appDbContext = context;
            _identityDbContext = identityDbContext;
            _config = config;
            _baseUrl = _config["BaseUrl"] ?? "https://default-url.com/";
        }

        public async Task<List<DoctorDto>?> GetTopTenDoctors()
        {
            var query = await _appDbContext.Doctors
                .Include(d => d.Specializations) // Include related specializations
                .Include(d => d.Reviews)        // Include related reviews
                .Include(d => d.AppointmentTypes)   // Include related AppointmentTypes
                .Select(d => new
                {
                    d.Id,
                    d.AppUserId, // Fetch AppUserId for linking to the Users table
                    Types = d.AppointmentTypes.Select(x=> new AppointmentTypeResponse
                    {
                        Id = x.Id.ToString(),
                        VisitType = x.VisitType,
                        ConsultationFee = x.ConsultationFee,
                        Duration = x.Duration,
                    })
                    .ToList()
                    ,
                    d.YearOfExperience,
                    Specializations = d.Specializations.Select(s => new SpecializationResponse
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Category = s.Category,
                    })
                    .ToList(),
                    AverageRate = d.Reviews.Any() ? d.Reviews.Average(r => r.Rate) : 0, // Pre-calculate average rate
                    ReviewCount = d.Reviews.Count(),
                })
                .OrderByDescending(d => d.AverageRate)
                .ThenByDescending(d => d.ReviewCount)
                .Take(10)
                .ToListAsync();

           if(query is not null)
            {
                // Fetch user details using AppUserId
                var appUserIds = query.Select(d => d.AppUserId).ToList();

                var userDetails = await _identityDbContext.Users
                    .Where(u => appUserIds.Contains(u.Id)) // Match AppUserId
                    .Select(u => new
                    {
                        u.Id,
                        FullName = u.FirstName + " " + u.LastName,
                        u.ImagePath
                    })
                    .ToDictionaryAsync(u => u.Id, u => new { u.FullName, u.ImagePath });

                // Map the final result
                return query.Select(d => new DoctorDto
                {
                    Id = d.Id.ToString(),
                    FullName = userDetails.ContainsKey(d.AppUserId) ? userDetails[d.AppUserId].FullName : "Unknown",
                    ImagePath = userDetails.ContainsKey(d.AppUserId) ? $"{_baseUrl}{userDetails[d.AppUserId].ImagePath}" : $"{_baseUrl}default.jpg",
                    AppointmentTypes = d.Types,
                    YearOfExperience = d.YearOfExperience,
                    Specializations = d.Specializations.ToList(),
                    NumberOfReviews = d.ReviewCount,
                })
                .ToList();
            }

            return null!;
        }

        public async Task<Dictionary<int, (string FullName, string ImagePath)>> GetUserDetailsAsync(IEnumerable<int> appUserIds)
        {
            return await _identityDbContext.Users
                .Where(u => appUserIds.Contains(u.Id)) // Match AppUserId
                .Select(u => new
                {
                    u.Id,
                    FullName = u.FirstName + " " + u.LastName,
                    u.ImagePath
                })
                .ToDictionaryAsync(u => u.Id, u => (u.FullName!, u.ImagePath!));
        }

        public async Task<Dictionary<int, (string FullName, string ImagePath)>> GetUserDetailsAsync(int appUserId)
        {
            return await _identityDbContext.Users
                .Where(u => u.Id == appUserId) // Match AppUserId
                .Select(u => new
                {
                    u.Id,
                    FullName = u.FirstName + " " + u.LastName,
                    u.ImagePath
                })
                .ToDictionaryAsync(u => u.Id, u => (u.FullName!, u.ImagePath!));
        }

        // I did it for better performance to select specific columns that I need it only...


        public async Task<List<Doctor>> GetDoctors()
        {
            var query = await _appDbContext.Doctors.Include(i => i.WorkingTimes)
                                                   .Include(i => i.Reviews)
                                                   .Include(i => i.ClinicAddresses)
                                                   .Include(i => i.Specializations)
                                                   .Include(i => i.AppointmentTypes)
                                                   .Select(x => new Doctor
                                                   {
                                                       Id = x.Id,
                                                       AboutMe = x.AboutMe,
                                                       AppUserId = x.AppUserId,
                                                       LicenseNumber = x.LicenseNumber,
                                                       NumberOfReviews = x.NumberOfReviews,
                                                       YearOfExperience = x.YearOfExperience,
                                                       AppointmentTypes = x.AppointmentTypes 
                                                       .Select(a=> new AppointmentType
                                                       {
                                                           Id = a.Id,
                                                           Duration = a.Duration,
                                                           ConsultationFee = a.ConsultationFee,
                                                           DoctorId = a.DoctorId,
                                                           VisitType = a.VisitType,
                                                       })
                                                       .ToList(),
                                                       Reviews = x.Reviews
                                                       .Select(r => new Review
                                                       {
                                                           Id = r.Id,
                                                           Rate = r.Rate,
                                                           Comment = r.Comment,
                                                           AppUserId = r.AppUserId,
                                                       })
                                                       .ToList(),
                                                       ClinicAddresses = x.ClinicAddresses
                                                       .Select(a => new ClinicAddress
                                                       {
                                                           Id = a.Id,
                                                           Street = a.Street,
                                                           City = a.City,
                                                           Country = a.Country,
                                                       })
                                                       .ToList(),
                                                       Specializations = x.Specializations
                                                       .Select(s => new Specialization
                                                       {
                                                           Id = s.Id,
                                                           Name = s.Name,
                                                           Category = s.Category,
                                                       })
                                                       .ToList(),
                                                       WorkingTimes = x.WorkingTimes
                                                       .Select(w => new WorkingTime
                                                       {
                                                           Id = w.Id,
                                                           DayOfWeek = w.DayOfWeek,
                                                           StartTime = w.StartTime,
                                                           EndTime = w.EndTime
                                                       })
                                                       .ToList(),
                                                   })
                                                   .ToListAsync();


            return query;
        }

        public async Task<Doctor> GetDoctorById(int doctorId)
        {
            var query = await _appDbContext.Doctors.Include(x => x.WorkingTimes)
                                                   .Include(x => x.Reviews)
                                                   .Include(x => x.ClinicAddresses)
                                                   .Include(x => x.Specializations)
                                                   .Include(x => x.AppointmentTypes)
                .Select(d => new Doctor
                {
                    Id = d.Id,
                    AboutMe = d.AboutMe,
                    AppUserId = d.AppUserId,
                    AppointmentTypes = d.AppointmentTypes,
                    LicenseNumber = d.LicenseNumber,
                    NumberOfReviews = d.NumberOfReviews,
                    YearOfExperience = d.YearOfExperience,
                    ClinicAddresses = d.ClinicAddresses.Select(x => new ClinicAddress
                    {
                        Id = x.Id,
                        Street = x.Street,
                        City = x.City,
                        Country = x.Country,
                    })
                .ToList(),
                    Reviews = d.Reviews.Select(x => new Review
                    {
                        Id = x.Id,
                        AppUserId = x.AppUserId,
                        Comment = x.Comment,
                        Rate = x.Rate,
                    })
                .ToList(),
                    Specializations = d.Specializations.Select(x => new Specialization
                    {
                        Id = x.Id,
                        Category = x.Category,
                        DoctorId = x.DoctorId,
                        Name = x.Name,
                    })
                .ToList(),
                    WorkingTimes = d.WorkingTimes.Select(x => new WorkingTime
                    {
                        Id = x.Id,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime,
                    })
                .ToList()
                })
                .FirstOrDefaultAsync(x => x.Id == doctorId);

            return query!;
        }

        public async Task<DoctorProfileResponse> GetDoctorProfile(int appUserIdFromClaims)
        {
            var query = await (
                from doctor in _appDbContext.Doctors
                join user in _identityDbContext.Users
                on doctor.AppUserId equals user.Id
                where doctor.AppUserId == appUserIdFromClaims
                select new DoctorProfileResponse
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? string.Empty,
                    ImageUrl = user.ImagePath ?? string.Empty,
                    PhoneNumber = user.PhoneNumber!,
                    YearsOfExperience = doctor.YearOfExperience,
                    Rating = doctor.Reviews.Any() ? doctor.Reviews.Average(r => r.Rate) : 0, // Handle empty reviews
                    Specialty = doctor.Specializations.Select(s => new SpecializationResponse
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Category = s.Category,
                    }).ToList(),
                    AppointmentTypes = doctor.AppointmentTypes.Select(a=> new AppointmentTypeResponse
                    {
                        Id = a.Id.ToString(),
                        ConsultationFee = a.ConsultationFee,
                        Duration = a.Duration,
                        VisitType = a.VisitType,
                    }).ToList(),
                    ClinicAddresses = doctor.ClinicAddresses.Select(c=> new ClinicAddressResponse
                    {
                        Id = c.Id,
                        City = c.City,
                        Country = c.Country,
                        Street = c.Street,  
                    }).ToList(),
                    WorkingTimes = doctor.WorkingTimes.Select(w=> new WorkingTimeResponse
                    {
                        Id = w.Id,
                        DayOfWeek = w.DayOfWeek,
                        DoctorId = w.DoctorId,
                        EndTime = w.EndTime,
                        StartTime = w.StartTime,
                    }).ToList(),
                  
                }
            ).FirstOrDefaultAsync();

            if(query is not null && query.ImageUrl is not null)
            {
                query.ImageUrl = $"{_baseUrl}{query.ImageUrl}";
            }

            return query!;
        }

        public async Task<List<DateTime>> GetAvailableTimeSlots(int doctorId, DateTime date, int appointmentTypeId)
        {
            // Fetch the doctor's working hours for the given day
            var workingTime = await _appDbContext.WorkingTimes
                .FirstOrDefaultAsync(w => w.DoctorId == doctorId && w.DayOfWeek == (DayOfWeek)date.DayOfWeek);

            if (workingTime == null)
                return new List<DateTime>(); // No working hours for the day

            var durationAppointmentType = await _appDbContext.AppointmentTypes
                                                             .Where(x=> x.DoctorId == doctorId && x.Id == appointmentTypeId)
                                                             .Select(x=> x.Duration)
                                                             .FirstOrDefaultAsync();
            if (durationAppointmentType == default)
                throw new BadRequest("Invalid Appointment Type.");


            // Convert TimeOnly duration to TimeSpan
            var slotDuration = durationAppointmentType.ToTimeSpan();

            // Fetch all appointments for the doctor on the given date
            var appointments = await _appDbContext.Appointments
                                                  .Where(a => a.DoctorId == doctorId && a.StartDate.Date == date.Date)
                                                  .Select(a => new { a.StartDate, a.EndDate, a.AppointmentType.Duration })
                                                  .ToListAsync();

        

            // Generate available time slots
            var availableSlots = new List<DateTime>();

            // Combine the date with the working hours' start and end times
            var startTime = date.Date.Add(workingTime.StartTime.ToTimeSpan());
            var endTime = date.Date.Add(workingTime.EndTime.ToTimeSpan());

            for (var time = startTime; time < endTime; time = time.Add(slotDuration))
            {
                var slotStart = time;
                var slotEnd = slotStart + slotDuration;

                // Ensure the slot does not exceed the doctor's working hours
                if (slotEnd > endTime)
                    break;

                // Check if the slot overlaps with any existing appointments
                bool isSlotAvailable = !appointments.Any(a =>
                    a.StartDate < slotEnd && a.EndDate > slotStart);

                if (isSlotAvailable)
                {
                    availableSlots.Add(slotStart);
                }
            }

            return availableSlots;
        }

        public async Task<List<AppointmentType>> GetAppointmentTypes(int doctorId)
        {
            return await _appDbContext.AppointmentTypes.ToListAsync();
        }
    }
}
