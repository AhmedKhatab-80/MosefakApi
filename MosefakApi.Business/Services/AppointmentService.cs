namespace MosefakApi.Business.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public AppointmentService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, ICacheService cacheService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppointmentResponse>?> UpcomingAppointments(int userIdFromClaims)
        {
            var appointments = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().GetAppointments(x => 
                                                              x.AppointmentStatus != AppointmentStatus.Completed &&
                                                              x.AppointmentStatus != AppointmentStatus.CanceledByPatient &&
                                                              x.AppointmentStatus != AppointmentStatus.CanceledByDoctor &&
                                                              x.PatientId == userIdFromClaims &&
                                                              x.EndDate >= DateTime.UtcNow);

            return await ProcessAppointments(appointments);
        }

        public async Task<IEnumerable<AppointmentResponse>?> CanceledAppointments(int userIdFromClaims)
        {
            var appointments = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().GetAppointments(x => 
                                                              x.AppointmentStatus != AppointmentStatus.Completed &&
                                                              x.AppointmentStatus == AppointmentStatus.CanceledByPatient &&
                                                              x.AppointmentStatus == AppointmentStatus.CanceledByDoctor &&
                                                              x.PatientId == userIdFromClaims);
          
            return await ProcessAppointments(appointments);
        }


        public async Task<IEnumerable<AppointmentResponse>?> CompletedAppointments(int userIdFromClaims)
        {
            var appointments = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().GetAppointments(x =>
                                                             x.AppointmentStatus == AppointmentStatus.Completed &&
                                                             x.AppointmentStatus != AppointmentStatus.CanceledByPatient &&
                                                             x.AppointmentStatus != AppointmentStatus.CanceledByDoctor &&
                                                             x.PatientId == userIdFromClaims);

            return await ProcessAppointments(appointments);
        }

        public async Task<bool> CancelAppointmentByPatient(int appointmentId, string? cancelationReason)
        {
            var appointment = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>()
                                               .FirstOrDefaultASync(x => x.Id == appointmentId, ["Payment"]);

            if (appointment == null || appointment.AppointmentStatus == AppointmentStatus.Completed)
                throw new BadRequest("Cannot cancel a completed appointment.");

            if (!string.IsNullOrEmpty(cancelationReason))
                appointment.CancellationReason = cancelationReason;

            appointment.AppointmentStatus = AppointmentStatus.CanceledByPatient;
            appointment.CancelledAt = DateTime.UtcNow;

            if (appointment.Payment?.Status == PaymentStatus.Paid)
            {
                appointment.Payment.Status = PaymentStatus.Refunded;
                //_stripeService.RefundPayment(appointment.Payment.StripePaymentIntentId);
            }

            await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().UpdateEntityAsync(appointment);
            await _unitOfWork.CommitAsync();

            // TODO: Send Notification To Doctor to tell him that This Patient canceled his appointment..

            // _logger.LogInformation($"Appointment {appointmentId} was cancelled by user {appointment.AppUserId}.");

            await _cacheService.RemoveCachedResponseAsync("/api/Appointments/canceled-appointments");

            return true;
        }

        // StartDate must be before EndDate in reschudle.
        public async Task<bool> RescheduleAppointmentAsync(int appointmentId, DateTimeOffset newStartDate)
        {
            var appointment = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().GetByIdAsync(appointmentId);

            if (appointment is null)
                throw new ItemNotFound("not exist appointment");

            if (newStartDate < DateTime.Now)
                throw new BadRequest("Can't reschedule in the past");

            if (appointment.AppointmentStatus == AppointmentStatus.Completed || appointment.AppointmentStatus == AppointmentStatus.CanceledByPatient)
                throw new BadRequest("Can't reschedule canceled or completed appointment!");


            // Calculate the new end date (assuming a fixed duration, e.g., 30 minutes)
            var newEndDate = newStartDate.AddMinutes(30);

            // Check if the new time slot is available
            var isSlotAvailable = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().IsTimeSlotAvailable(appointment.DoctorId, newStartDate, newEndDate);

            if (!isSlotAvailable)
            {
                throw new InvalidOperationException("The selected time slot is no longer available.");
            }

            // Update the appointment
            appointment.StartDate = newStartDate;
            appointment.EndDate = newEndDate;
            appointment.AppointmentStatus = AppointmentStatus.PendingApproval;

            await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().UpdateEntityAsync(appointment);
            
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
                var doctor = await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().FirstOrDefaultASync(x => x.Id == request.DoctorId, ["AppointmentTypes"]);

                if (doctor == null)
                    throw new ItemNotFound("Doctor is not exist");

                var appointmentType = doctor.AppointmentTypes.FirstOrDefault(a => a.Id == request.AppointmentTypeId);

                if (appointmentType == null)
                    throw new ItemNotFound("this appointment type is not exist for this doctor");

                var endTime = new DateTimeOffset();

                if (appointmentType.Duration.Hours == 0 && appointmentType.Duration.Minutes > 0)
                    endTime = request.StartDate.AddMinutes(appointmentType.Duration.Minutes); 

                if(appointmentType.Duration.Hours > 0 && appointmentType.Duration.Minutes > 0)
                    endTime = request.StartDate.Add(new TimeSpan(appointmentType.Duration.Hours, appointmentType.Duration.Minutes, 0));

                if (!await IsTimeSlotAvailable(doctor.Id, request.StartDate, endTime))
                    throw new BadRequest("Can't book appointment already booked!, try another time");

                var appointment = new Appointment()
                {
                    PatientId = appUserIdFromClaims,
                    DoctorId = request.DoctorId,
                    AppointmentStatus = AppointmentStatus.PendingApproval,
                    AppointmentTypeId = request.AppointmentTypeId,
                    PaymentStatus = PaymentStatus.Pending,
                    StartDate = request.StartDate,
                    ProblemDescription = request.ProblemDescription,
                    EndDate = endTime,
                };

                // Add the appointment and commit the transaction

                await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().AddEntityAsync(appointment);
                
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

        public async Task<bool> IsTimeSlotAvailable(int doctorId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var query = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().IsTimeSlotAvailable(doctorId, startDate, endDate);

            return query;
        }

        public async Task<bool> ApproveAppointmentByDoctor(int appointmentId)
        {
            var appointment = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().GetByIdAsync(appointmentId);

            if (appointment == null)
                throw new ItemNotFound("appointment is not exist");

            appointment.AppointmentStatus = AppointmentStatus.PendingPayment;
            appointment.ApprovedByDoctor = true;
            appointment.PaymentDueTime = DateTime.Now.AddHours(24);

            await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().UpdateEntityAsync(appointment);
            await _unitOfWork.CommitAsync();

            // TODO: Send Notification To Patient to tell him that doctor approve appointment and must pay during 24 Hours for example.
            
            return true;
        }

        public async Task<bool> RejectAppointmentByDoctor(int appointmentId, string? rejectionReason)
        {
            var appointment = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().GetByIdAsync(appointmentId);

            if (appointment == null || appointment.AppointmentStatus != AppointmentStatus.PendingApproval)
                throw new BadRequest("Invalid appointment or already processed");

            appointment.AppointmentStatus = AppointmentStatus.CanceledByDoctor;
            appointment.CancelledAt = DateTime.UtcNow;

            if (rejectionReason != null)
                appointment.CancellationReason = rejectionReason;

            await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().UpdateEntityAsync(appointment);
            await _unitOfWork.CommitAsync();

            // TODO: Send Notification To Patient to tell him that.
            
            return true;
        }

        public async Task<bool> Pay(int appointmentId)
        {
            var appointment = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().FirstOrDefaultASync(x => x.Id == appointmentId, ["AppointmentType", "Payment"]);

            if (appointment == null || appointment.AppointmentStatus != AppointmentStatus.PendingPayment)
                throw new BadRequest("Invalid appointment");

            decimal amountToCharge = appointment.AppointmentType.ConsultationFee;

            // TODO: Genegrate PaymentIntentId By PaymentService

            var payment = new Payment
            {
                AppointmentId = appointmentId,
                StripePaymentIntentId = Guid.NewGuid().ToString(), // currently only
                Amount = amountToCharge,
                Status = PaymentStatus.Paid,
            };

            appointment.AppointmentStatus = AppointmentStatus.Confirmed;
            appointment.Payment = payment;

            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> MarkAppointmentAsCompleted(int appointmentId)
        {
            var appointment = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().GetByIdAsync(appointmentId);

            if (appointment == null || appointment.AppointmentStatus != AppointmentStatus.Confirmed)
                throw new BadRequest("Invalid appointment");

            appointment.AppointmentStatus = AppointmentStatus.Completed;
            appointment.CompletedAt = DateTime.UtcNow;
            appointment.ServiceProvided = true;

            await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().UpdateEntityAsync(appointment);
            await _unitOfWork.CommitAsync();

            // TODO: Send Notification To Patient to tell him that appointment completed successfully.

            return true;
        }


        public async Task<bool> CancelAppointmentByDoctor(int appointmentId, string? cancelationReason)
        {
            var appointment = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().FirstOrDefaultASync(x=> x.Id == appointmentId, ["Payment"]);

            if (appointment == null || appointment.AppointmentStatus != AppointmentStatus.Confirmed)
                throw new BadRequest("Invalid appointment.");

            appointment.AppointmentStatus = AppointmentStatus.CanceledByDoctor;
            appointment.CancelledAt = DateTime.UtcNow;

            if (cancelationReason != null)
                appointment.CancellationReason = cancelationReason;

            if (appointment.Payment?.Status == PaymentStatus.Paid)
            {
                appointment.Payment.Status = PaymentStatus.Refunded;

                //_stripeService.RefundPayment(appointment.Payment.StripePaymentIntentId);
            }

            await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().UpdateEntityAsync(appointment);
            await _unitOfWork.CommitAsync();

            // TODO: Send Notification To Patient to tell him that.
            // TODO: Refund Money To Patinet by PaymenetService 

            return true;
        }

        public async Task AutoCancelUnpaidAppointments()
        {
            Expression<Func<Appointment, bool>> expression = x => x.PaymentStatus == PaymentStatus.Pending &&
                                                                  x.AppointmentStatus == AppointmentStatus.PendingPayment &&
                                                                  x.PaymentDueTime < DateTime.Now;

            var expiredAppointments = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().GetAllAsync(expression);

            if (expiredAppointments == null)
                return;

            foreach (var appointment in expiredAppointments)
            {
                appointment.AppointmentStatus = AppointmentStatus.AutoCanceled;
                appointment.CancelledAt = DateTime.UtcNow;
                appointment.CancellationReason = "Payment not completed in time";
            }

            await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().UpdateRangeAsync(expiredAppointments);
            await _unitOfWork.CommitAsync();

            // TODO: Send Notifications To Patients that has these appointments after auto canceled (24 Hour)
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

