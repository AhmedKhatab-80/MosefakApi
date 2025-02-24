namespace MosefakApp.Infrastructure.Repositories.Non_Generic
{
    public class AppointmentRepositoryAsync : GenericRepositoryAsync<Appointment>, IAppointmentRepositoryAsync
    {
        private readonly AppDbContext _context;
        public AppointmentRepositoryAsync(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AppointmentResponse>> GetAppointments(Expression<Func<Appointment, bool>> expression)
        {
            var appointments = await _context.Appointments
                                                   .Include(x=> x.AppointmentType)
                                                   .Include(x => x.Doctor)
                                                   .ThenInclude(x => x.Specializations)
                                                   .Where(expression)
                                                   .Select(x =>
                                                                new AppointmentResponse
                                                                {
                                                                    Id = x.Id.ToString(),
                                                                    DoctorId = x.Doctor.AppUserId,
                                                                    StartDate = x.StartDate,
                                                                    EndDate = x.EndDate,
                                                                    AppointmentType = new AppointmentTypeResponse
                                                                    {
                                                                        Id = x.AppointmentType.Id.ToString(),
                                                                        ConsultationFee = x.AppointmentType.ConsultationFee,
                                                                        Duration = x.AppointmentType.Duration,
                                                                        VisitType = x.AppointmentType.VisitType
                                                                    },
                                                                    DoctorSpecialization = x.Doctor.Specializations.Select(s => new SpecializationResponse
                                                                    {
                                                                        Id = s.Id.ToString(),
                                                                        Name = s.Name,
                                                                        Category = s.Category,
                                                                    })
                                                       .ToList()
                                                                })
                                                   .ToListAsync();

            return appointments;
        }

        public async Task<bool> IsTimeSlotAvailable(int doctorId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var overlappingAppointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                            a.StartDate < endDate &&
                            a.EndDate > startDate)
                .ToListAsync();

            return !overlappingAppointments.Any();
        }
    }
}
