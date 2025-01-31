namespace MosefakApi.Business.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork<Appointment> _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public AppointmentService(IUnitOfWork<Appointment> unitOfWork, UserManager<AppUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppointmentResponse>?> UpcomingAppointments(int userIdFromClaims)
        {
            var appointments = await _unitOfWork.AppointmentRepositoryAsync.GetAppointments(x => 
                                                              x.AppointmentStatus != AppointmentStatus.Completed &&
                                                              x.AppointmentStatus != AppointmentStatus.Cancelled &&
                                                              x.AppUserId == userIdFromClaims &&
                                                              x.EndDate >= DateTime.UtcNow);

            return await ProcessAppointments(appointments);
        }

        public async Task<IEnumerable<AppointmentResponse>?> CanceledAppointments(int userIdFromClaims)
        {
            var appointments = await _unitOfWork.AppointmentRepositoryAsync.GetAppointments(x => 
                                                              x.AppointmentStatus != AppointmentStatus.Completed &&
                                                              x.AppointmentStatus == AppointmentStatus.Cancelled &&
                                                              x.AppUserId == userIdFromClaims);
          
            return await ProcessAppointments(appointments);
        }


        public async Task<IEnumerable<AppointmentResponse>?> CompletedAppointments(int userIdFromClaims)
        {
            var appointments = await _unitOfWork.AppointmentRepositoryAsync.GetAppointments(x =>
                                                             x.AppointmentStatus == AppointmentStatus.Completed &&
                                                             x.AppointmentStatus != AppointmentStatus.Cancelled &&
                                                             x.AppUserId == userIdFromClaims);

            return await ProcessAppointments(appointments);
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId, string? cancelationReason)
        {
            var appointment = await _unitOfWork.AppointmentRepositoryAsync
                                               .FirstOrDefaultASync(x =>
                                                                         x.Id == appointmentId &&
                                                                         x.AppointmentStatus != AppointmentStatus.Completed &&
                                                                         x.AppointmentStatus != AppointmentStatus.Cancelled);

            if (appointment is null)
                throw new ItemNotFound("Appointment not found or cannot be cancelled.");

            if (!string.IsNullOrEmpty(cancelationReason))
                appointment.CancellationReason = cancelationReason;

            appointment.AppointmentStatus = AppointmentStatus.Cancelled;

            await _unitOfWork.AppointmentRepositoryAsync.UpdateEntityAsync(appointment);
            await _unitOfWork.CommitAsync();

           // _logger.LogInformation($"Appointment {appointmentId} was cancelled by user {appointment.AppUserId}.");

            return true;
        }

        // StartDate must be before EndDate in reschudle.
        public Task<bool> RescheduleAppointmentAsync(int appointmentId, DateTime newDateTime)
        {
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<AppointmentResponse>?> ProcessAppointments(IEnumerable<AppointmentResponse>? appointments)
        {
            if (appointments is null || !appointments.Any())
                return null;

            var doctorIds = GetDoctorIds(appointments);
            var doctors = await GetDoctors(doctorIds);
            CompleteMap(appointments, doctors);

            return appointments;
        }

        private IEnumerable<int> GetDoctorIds(IEnumerable<AppointmentResponse> appointments)
        {
            return appointments.Select(a => a.DoctorId).Distinct().ToList();
        }

        private async Task<Dictionary<int, DoctorDetails>> GetDoctors(IEnumerable<int> doctorIds)
        {
            return await _userManager.Users
                .Where(u => doctorIds.Contains(u.Id))
                .Select(u => new DoctorDetails
                {
                    Id = u.Id,
                    FirstName = u.FirstName ?? "Unknown", // Handle nulls
                    LastName = u.LastName ?? "Unknown", 
                    ImagePath = u.ImagePath ?? null!
                })
                .ToDictionaryAsync(u => u.Id);
        }

        private void CompleteMap(IEnumerable<AppointmentResponse> appointments, Dictionary<int, DoctorDetails> doctors)
        {
            foreach (var appointment in appointments)
            {
                if (doctors.TryGetValue(appointment.DoctorId, out var doctor))
                {
                    _mapper.Map(doctor, appointment); // need optimize for image
                }
                else
                {
                    throw new ItemNotFound($"Doctor with ID {appointment.DoctorId} not found.");
                }
            }
        }

    }
}

