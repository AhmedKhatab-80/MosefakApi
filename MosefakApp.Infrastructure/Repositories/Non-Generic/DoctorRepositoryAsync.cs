namespace MosefakApp.Infrastructure.Repositories.Non_Generic
{
    public class DoctorRepositoryAsync : GenericRepositoryAsync<Doctor>, IDoctorRepositoryAsync
    {
        private readonly AppDbContext _appDbContext;
        private readonly AppIdentityDbContext _identityDbContext;
        public DoctorRepositoryAsync(AppDbContext context, AppIdentityDbContext identityDbContext) : base(context)
        {
            _appDbContext = context;
            _identityDbContext = identityDbContext;
        }

        public async Task<IList<DoctorDto>> GetTopTenDoctors()
        {
            var query = await _appDbContext.Doctors
                .Include(d => d.Specializations) // Include related specializations
                .Include(d => d.Reviews)        // Include related reviews
                .Select(d => new
                {
                    d.Id,
                    d.AppUserId, // Fetch AppUserId for linking to the Users table
                    d.ConsultationFee,
                    d.YearOfExperience,
                    Specializations = d.Specializations.Select(s => new SpecializationResponse
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Category = s.Category,
                    }),
                    AverageRate = d.Reviews.Any() ? d.Reviews.Average(r => r.Rate) : 0, // Pre-calculate average rate
                    ReviewCount = d.Reviews.Count(),
                })
                .OrderByDescending(d => d.AverageRate)
                .ThenByDescending(d => d.ReviewCount)
                .Take(10)
                .ToListAsync();

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
                Id = d.Id,
                FullName = userDetails.ContainsKey(d.AppUserId) ? userDetails[d.AppUserId].FullName : "Unknown",
                ImagePath = userDetails.ContainsKey(d.AppUserId) ? userDetails[d.AppUserId].ImagePath! : null!,
                ConsultationFee = d.ConsultationFee,
                YearOfExperience = d.YearOfExperience,
                Specializations = d.Specializations.ToList(),
                NumberOfReviews = d.ReviewCount,
            })
            .ToList();
        }

        public async Task<Dictionary<int, (string FullName, string ImagePath)>> GetUserDetailsAsync(List<int> appUserIds)
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


        public async Task<IList<Doctor>> GetDoctors()
        {
            var query = await _appDbContext.Doctors.Include(i => i.WorkingTimes)
                                                   .Include(i => i.Reviews)
                                                   .Include(i => i.ClinicAddresses)
                                                   .Include(i => i.Specializations)
                                                   .Select(x => new Doctor
                                                   {
                                                       Id = x.Id,
                                                       AboutMe = x.AboutMe,
                                                       AppUserId = x.AppUserId,
                                                       ConsultationFee = x.ConsultationFee,
                                                       LicenseNumber = x.LicenseNumber,
                                                       NumberOfReviews = x.NumberOfReviews,
                                                       YearOfExperience = x.YearOfExperience,
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
            var query = await _appDbContext.Doctors.Include(x=> x.WorkingTimes)
                                                   .Include(x => x.Reviews)
                                                   .Include(x => x.ClinicAddresses)
                                                   .Include(x => x.Specializations)
                .Select(d => new Doctor
            {
                Id = d.Id,
                AboutMe = d.AboutMe,
                AppUserId = d.AppUserId,
                ConsultationFee = d.ConsultationFee,
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
                .FirstOrDefaultAsync(x=> x.Id == doctorId);

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
                    YearsOfExperience = doctor.YearOfExperience,
                    Rating = doctor.Reviews.Any() ? doctor.Reviews.Average(r => r.Rate) : 0, // Handle empty reviews
                    Specialty = doctor.Specializations.Select(s => new SpecializationResponse
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Category = s.Category,
                    }).ToList(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? string.Empty,
                    ImagePath = user.ImagePath ?? string.Empty,
                    PhoneNumber = user.PhoneNumber!
                }
            ).FirstOrDefaultAsync();

            return query!;
        }

    }
}
