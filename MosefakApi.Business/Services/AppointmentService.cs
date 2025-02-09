namespace MosefakApi.Business.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork<Appointment> _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public AppointmentService(IUnitOfWork<Appointment> unitOfWork, UserManager<AppUser> userManager, ICacheService cacheService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _cacheService = cacheService;
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

            await _cacheService.RemoveCachedResponseAsync("/api/Appointments/canceled-appointments");

            return true;
        }

        // StartDate must be before EndDate in reschudle.
        public async Task<bool> RescheduleAppointmentAsync(int appointmentId, DateTime newStartDate)
        {
            var appointment = await _unitOfWork.AppointmentRepositoryAsync.GetByIdAsync(appointmentId);

            if (appointment is null)
                throw new ItemNotFound("not exist appointment");

            if (newStartDate < DateTime.Now)
                throw new BadRequest("Can't reschedule in the past");

            if (appointment.AppointmentStatus == AppointmentStatus.Completed || appointment.AppointmentStatus == AppointmentStatus.Cancelled)
                throw new BadRequest("Can't reschedule canceled or completed appointment!");


            // Calculate the new end date (assuming a fixed duration, e.g., 30 minutes)
            var newEndDate = newStartDate.AddMinutes(30);

            // Check if the new time slot is available
            var isSlotAvailable = await _unitOfWork.AppointmentRepositoryAsync.IsTimeSlotAvailable(appointment.DoctorId, newStartDate, newEndDate);

            if (!isSlotAvailable)
            {
                throw new InvalidOperationException("The selected time slot is no longer available.");
            }

            // Update the appointment
            appointment.StartDate = newStartDate;
            appointment.EndDate = newEndDate;
            appointment.AppointmentStatus = AppointmentStatus.Rescheduled;

            await _unitOfWork.RepositoryAsync.UpdateEntityAsync(appointment);
            
            var rowAffected = await _unitOfWork.CommitAsync();

            if(rowAffected < 0)
                return false;
            else
            {
                // log $"Appointment {Id} rescheduled from {StartDate} to {newStartDate}."
                await _cacheService.RemoveCachedResponseAsync("/api/appointments/upcoming-appointments");

                return true;
            }
        }

        public async Task<bool> BookAppointment(BookAppointmentRequest request, int appUserIdFromClaims)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var doctor = await _unitOfWork.DoctorRepositoryAsync.FirstOrDefaultASync(x => x.Id == request.DoctorId, ["AppointmentTypes"]);

                if (doctor == null)
                    throw new ItemNotFound("Doctor is not exist");

                var appointmentType = doctor.AppointmentTypes.FirstOrDefault(a => a.Id == request.AppointmentTypeId);

                if (appointmentType == null)
                    throw new ItemNotFound("this appointment type is not exist for this doctor");

                var endTime = new DateTime();

                if (appointmentType.Duration.Hour == 0 && appointmentType.Duration.Minute > 0)
                    endTime = request.StartDate.AddMinutes(appointmentType.Duration.Minute); 

                if(appointmentType.Duration.Hour > 0 && appointmentType.Duration.Minute > 0)
                    endTime = request.StartDate.Add(new TimeSpan(appointmentType.Duration.Hour, appointmentType.Duration.Minute, 0));

                if (!await IsTimeSlotAvailable(doctor.Id, request.StartDate, endTime))
                    throw new BadRequest("Can't book appointment already booked!, try another time");

                var appointment = new Appointment()
                {
                    AppUserId = appUserIdFromClaims,
                    DoctorId = request.DoctorId,
                    AppointmentStatus = AppointmentStatus.Scheduled,
                    AppointmentTypeId = request.AppointmentTypeId,
                    PaymentStatus = PaymentStatus.Pending,
                    StartDate = request.StartDate,
                    ProblemDescription = request.ProblemDescription,
                    EndDate = endTime,
                };

                // Add the appointment and commit the transaction

                await _unitOfWork.AppointmentRepositoryAsync.AddEntityAsync(appointment);
                
                var rowsAffected = await _unitOfWork.CommitAsync();

                await transaction.CommitAsync(); // ✅ Commit only if successful

                await _cacheService.RemoveCachedResponseAsync("/api/appointments/upcoming-appointments");

                return rowsAffected > 0;
            }
            catch
            {
                await transaction.RollbackAsync(); // ✅ Rollback in case of failure
                throw;
            }
        }

        public async Task<bool> IsTimeSlotAvailable(int doctorId, DateTime startDate, DateTime endDate)
        {
            var query = await _unitOfWork.AppointmentRepositoryAsync.IsTimeSlotAvailable(doctorId, startDate, endDate);

            return query;
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

