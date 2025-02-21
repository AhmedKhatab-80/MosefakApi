namespace MosefakApi.Business.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICacheService _cacheService;
        private readonly IStripeService _stripeService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILoggerService _loggerService;
        private readonly string _baseUrl;

        public AppointmentService(
            IUnitOfWork unitOfWork, UserManager<AppUser> userManager, ICacheService cacheService,
            IStripeService stripeService, IMapper mapper, ILoggerService loggerService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _cacheService = cacheService;
            _stripeService = stripeService;
            _mapper = mapper;
            _configuration = configuration;
            _loggerService = loggerService;
            _baseUrl = _configuration["BaseUrl"] ?? "https://default-url.com/";
        }

        public async Task<List<AppointmentResponse>> GetPatientAppointments(int userIdFromClaims, AppointmentStatus? status = null, CancellationToken cancellationToken = default)
        {
            var appointments = await FetchPatientAppointments(userIdFromClaims, status);
            if (!appointments.Any()) return new List<AppointmentResponse>();

            var doctorAppUserIds = appointments.Select(a => a.Doctor.AppUserId).Distinct().ToList();
            var doctorDetails = await FetchDoctorDetails(doctorAppUserIds, cancellationToken);

            return appointments.Select(a => MapAppointmentResponse(a, doctorDetails)).ToList();
        }

        public async Task<List<AppointmentResponse>> GetDoctorAppointments(int doctorId, AppointmentStatus? status = null, CancellationToken cancellationToken = default)
        {
            var appointments = await FetchPatientAppointments(doctorId, status);
            if (!appointments.Any()) return new List<AppointmentResponse>();

            var doctorAppUserIds = appointments.Select(a => a.Doctor.AppUserId).Distinct().ToList();

            var doctorDetails = await FetchDoctorDetails(doctorAppUserIds, cancellationToken);

            return appointments.Select(a => MapAppointmentResponse(a, doctorDetails)).ToList();
        }
        public async Task<AppointmentResponse> GetAppointmentById(int appointmentId, CancellationToken cancellationToken = default)
        {
            var appointment = await _unitOfWork.Repository<Appointment>()
                .FirstOrDefaultASync(x => x.Id == appointmentId, ["AppointmentType", "Doctor", "Doctor.Specializations"]);

            if (appointment == null) return new AppointmentResponse();

            var doctorDetails = await FetchDoctorDetails(new List<int> { appointment.Doctor.AppUserId }, cancellationToken);

            return MapAppointmentResponse(appointment, doctorDetails);
        }

        public async Task<bool> CancelAppointmentByPatient(int patientId, int appointmentId, string? cancellationReason, CancellationToken cancellationToken = default)
        {
            // 1️⃣ Validate patient existence.
            var patient = await _userManager.Users
                .FirstOrDefaultAsync(x => x.Id == patientId, cancellationToken)
                .ConfigureAwait(false);

            if (patient is null)
            {
                _loggerService.LogWarning("User with Id {PatientId} does not exist.", patientId);
                throw new ItemNotFound("User does not exist.");
            }

            // 2️⃣ Retrieve the appointment including Payment navigation.
            var appointment = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>()
                .FirstOrDefaultASync(x => x.Id == appointmentId, ["Payment"])
                .ConfigureAwait(false);

            if (appointment == null)
            {
                _loggerService.LogWarning("Appointment with Id {AppointmentId} not found.", appointmentId);
                throw new ItemNotFound("Appointment does not exist.");
            }

            // 3️⃣ Validate that the appointment is not completed.
            if (appointment.AppointmentStatus == AppointmentStatus.Completed)
            {
                throw new BadRequest("Cannot cancel a completed appointment.");
            }

            // 4️⃣ Set cancellation reason if provided.
            if (!string.IsNullOrWhiteSpace(cancellationReason))
            {
                appointment.CancellationReason = cancellationReason;
            }

            // 5️⃣ Update appointment status and cancellation timestamp.
            appointment.AppointmentStatus = AppointmentStatus.CanceledByPatient;
            appointment.CancelledAt = DateTimeOffset.UtcNow;

            // 6️⃣ Handle payment refund if applicable.
            if (appointment.Payment?.Status == PaymentStatus.Paid)
            {
                appointment.Payment.Status = PaymentStatus.Refunded;
                // TODO: Invoke payment refund service (e.g., _stripeService.RefundPayment(..., cancellationToken))
                // TODO: Send notification to the patient regarding the refund.
            }

            // 7️⃣ Update the appointment in the repository and commit changes.
            await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>()
                .UpdateEntityAsync(appointment)
                .ConfigureAwait(false);

            var rowsAffected = await _unitOfWork.CommitAsync(cancellationToken)
                .ConfigureAwait(false);

            if (rowsAffected <= 0)
            {
                _loggerService.LogError("Failed to cancel appointment with Id {AppointmentId}.", appointmentId);
                throw new Exception("Failed to cancel appointment.");
            }

            // 8️⃣ Log cancellation and invalidate relevant caches.
            _loggerService.LogInfo("Appointment {AppointmentId} was cancelled by patient {PatientId}.", appointmentId, patientId);

            await _cacheService.RemoveCachedResponseAsync("/api/Appointments/canceled-appointments").ConfigureAwait(false);

            return true;
        }


        public async Task<List<AppointmentResponse>> GetAppointmentsByDateRange(
            int patientId, DateTimeOffset startDate, DateTimeOffset endDate,CancellationToken cancellationToken = default)
        {
            // Retrieve appointments with necessary navigations: AppointmentType, Doctor, and Doctor.Specializations.
            var appointments = await _unitOfWork.Repository<Appointment>()
                .GetAllAsync(
                    x => x.PatientId == patientId && x.CreatedAt >= startDate && x.CreatedAt <= endDate,
                    includes: ["AppointmentType", "Doctor", "Doctor.Specializations"]);

            if (!appointments.Any())
                return new List<AppointmentResponse>();

            // Extract distinct doctor AppUserIds from the appointments.
            var doctorAppUserIds = appointments
                .Select(a => a.Doctor.AppUserId)
                .Distinct()
                .ToList();

            // Fetch doctor details from the identity database.
            var doctorDetails = await FetchDoctorDetails(doctorAppUserIds, cancellationToken);

            // Map appointments to response DTOs.
            return appointments.Select(a => MapAppointmentResponse(a, doctorDetails)).ToList();
        }

        public async Task<List<AppointmentResponse>> GetAppointmentsByDateRangeForDoctor(
            int doctorId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
        {
            // Retrieve appointments with necessary navigations: AppointmentType, Doctor, and Doctor.Specializations.
            var appointments = await _unitOfWork.Repository<Appointment>()
                .GetAllAsync(
                    x => x.Doctor.AppUserId == doctorId && x.CreatedAt >= startDate && x.CreatedAt <= endDate,
                    includes: ["AppointmentType", "Doctor", "Doctor.Specializations"]);

            if (!appointments.Any())
                return new List<AppointmentResponse>();

            // Extract distinct doctor AppUserIds from the appointments.
            var doctorAppUserIds = appointments
                .Select(a => a.Doctor.AppUserId)
                .Distinct()
                .ToList();

            // Fetch doctor details from the identity database.
            var doctorDetails = await FetchDoctorDetails(doctorAppUserIds, cancellationToken);

            // Map appointments to response DTOs.
            return appointments.Select(a => MapAppointmentResponse(a, doctorDetails)).ToList();
        }


        // StartDate must be before EndDate in reschudle.
        public async Task<bool> RescheduleAppointmentAsync(int appointmentId, DateTime selectedDate, TimeSlot newTimeSlot)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var appointmentRepo = _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>();

            // 🔍 Fetch appointment
            var appointment = await appointmentRepo.FirstOrDefaultASync(x => x.Id == appointmentId);

            if (appointment is null)
                throw new ItemNotFound("Appointment does not exist.");

            if (appointment.AppointmentStatus == AppointmentStatus.Completed ||
                appointment.AppointmentStatus == AppointmentStatus.CanceledByPatient)
                throw new BadRequest("Cannot reschedule a canceled or completed appointment.");

            if (newTimeSlot.EndTime < newTimeSlot.StartTime)
                throw new BadRequest("Invalid time slot. End time must be after start time.");

            // 📅 Convert TimeSlot into UTC-based DateTimeOffsets
            var startTimeOffset = new DateTimeOffset(
                selectedDate.Year, selectedDate.Month, selectedDate.Day,
                newTimeSlot.StartTime.Hour, newTimeSlot.StartTime.Minute, 0,
                TimeZoneInfo.Local.GetUtcOffset(DateTime.Now));

            var endTimeOffset = startTimeOffset.Add(newTimeSlot.EndTime - newTimeSlot.StartTime);

            // ✅ **Check for availability**
            if (!await appointmentRepo.IsTimeSlotAvailable(appointment.DoctorId, startTimeOffset, endTimeOffset))
                throw new InvalidOperationException("The selected time slot is already booked.");

            // ✅ **Update appointment**
            appointment.StartDate = startTimeOffset;
            appointment.EndDate = endTimeOffset;
            appointment.AppointmentStatus = AppointmentStatus.PendingApproval;

            await appointmentRepo.UpdateEntityAsync(appointment);
            var rowsAffected = await _unitOfWork.CommitAsync();

            if (rowsAffected == 0)
                return false;

            // ✅ **Send notification to the doctor**
            //var doctorDeviceToken = await _unitOfWork.UserRepository.GetUserDeviceToken(appointment.DoctorId);
            //if (!string.IsNullOrEmpty(doctorDeviceToken))
            //{
            //    await _firebaseNotificationService.SendPushNotification(
            //        doctorDeviceToken,
            //        "Appointment Rescheduled",
            //        $"A patient has rescheduled an appointment to {startTimeOffset:dd/MM/yyyy HH:mm}.");
            //}

            // ✅ **Clear Cache**
            await _cacheService.RemoveCachedResponseAsync("/api/appointments/upcoming");

            transactionScope.Complete();
            return true;
        }


        public async Task<bool> BookAppointment(BookAppointmentRequest request, int appUserIdFromClaims, CancellationToken cancellationToken = default)
        {
            // Start a transaction.
            using var transaction = await _unitOfWork.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                // 1️⃣ Retrieve the doctor with their appointment types.
                var doctor = await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>()
                    .FirstOrDefaultASync(x => x.Id == request.DoctorId, new[] { "AppointmentTypes" })
                    .ConfigureAwait(false);

                if (doctor == null)
                    throw new ItemNotFound("Doctor does not exist.");

                // 2️⃣ Retrieve the specified appointment type.
                var appointmentType = doctor.AppointmentTypes.FirstOrDefault(a => a.Id.ToString() == request.AppointmentTypeId);
                if (appointmentType == null)
                    throw new ItemNotFound("This appointment type does not exist for this doctor.");

                // 3️⃣ Calculate the appointment's end time by adding the duration to the start date.
                // This handles all cases (hours, minutes, etc.) in one call.
                var endTime = request.StartDate.Add(appointmentType.Duration);

                // 4️⃣ Check if the new time slot is available.
                if (!await IsTimeSlotAvailable(doctor.Id, request.StartDate, endTime).ConfigureAwait(false))
                    throw new BadRequest("Cannot book appointment; the selected time slot is already booked. Please try another time.");

                // 5️⃣ Create the appointment.
                var appointment = new Appointment
                {
                    PatientId = appUserIdFromClaims,
                    DoctorId = request.DoctorId,
                    AppointmentStatus = AppointmentStatus.PendingApproval,
                    AppointmentTypeId = int.Parse(request.AppointmentTypeId),
                    PaymentStatus = PaymentStatus.Pending,
                    StartDate = request.StartDate,
                    EndDate = endTime,
                    ProblemDescription = !string.IsNullOrEmpty(request.ProblemDescription)? request.ProblemDescription : null,
                };

                await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>()
                    .AddEntityAsync(appointment)
                    .ConfigureAwait(false);

                var rowsAffected = await _unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
                if (rowsAffected <= 0)
                    throw new Exception("Failed to book appointment.");

                // 6️⃣ Commit the transaction.
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

                // TODO: Send Notofication to doctor tell him to approve

                // TODO: remove appointments cache.

                return true;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }


        private async Task<bool> IsTimeSlotAvailable(int doctorId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var query = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>().IsTimeSlotAvailable(doctorId, startDate, endDate);

            return query;
        }

        public async Task<bool> ApproveAppointmentByDoctor(int appointmentId)
        {
            var appointmentRepo = _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>();

            var appointment = await appointmentRepo.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                _loggerService.LogWarning($"Approval failed: Appointment {appointmentId} not found.");
                throw new ItemNotFound("Appointment does not exist.");
            }

            if (appointment.AppointmentStatus != AppointmentStatus.PendingApproval)
            {
                _loggerService.LogWarning($"Invalid status: Appointment {appointmentId} cannot be approved.");
                throw new BadRequest("Appointment is not in a pending approval state.");
            }

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                appointment.AppointmentStatus = AppointmentStatus.PendingPayment;
                appointment.ApprovedByDoctor = true;
                appointment.PaymentDueTime = DateTime.UtcNow.AddHours(24);

                await appointmentRepo.UpdateEntityAsync(appointment);
                await _unitOfWork.CommitAsync();

                //await _notificationService.SendNotificationToPatient(appointment.PatientId, "Appointment Approved",
                //    "Your appointment has been approved. Please complete payment within 24 hours.");

                await _unitOfWork.CommitTransactionAsync();
                _loggerService.LogInfo($"Appointment {appointmentId} approved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _loggerService.LogError($"Failed to approve appointment {appointmentId}: {ex.Message}");
                throw;
            }
        }
        public async Task<bool> RejectAppointmentByDoctor(int appointmentId, string? rejectionReason)
        {
            var appointmentRepo = _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>();
            var appointment = await appointmentRepo.GetByIdAsync(appointmentId);

            if (appointment == null || appointment.AppointmentStatus != AppointmentStatus.PendingApproval)
            {
                _loggerService.LogWarning($"Invalid rejection: Appointment {appointmentId} not found or already processed.");
                throw new BadRequest("Invalid appointment or already processed.");
            }

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                appointment.AppointmentStatus = AppointmentStatus.CanceledByDoctor;
                appointment.CancelledAt = DateTime.UtcNow;
                appointment.CancellationReason = rejectionReason;

                await appointmentRepo.UpdateEntityAsync(appointment);
                await _unitOfWork.CommitAsync();

                //await _notificationService.SendNotificationToPatient(appointment.PatientId, "Appointment Rejected",
                //    "Your appointment request was rejected. Reason: " + (rejectionReason ?? "Not specified."));

                await _unitOfWork.CommitTransactionAsync();
                _loggerService.LogInfo($"Appointment {appointmentId} rejected successfully.");
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _loggerService.LogError($"Failed to reject appointment {appointmentId}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<AppointmentResponse>> GetPendingAppointmentsForDoctor(int doctorId, CancellationToken cancellationToken = default)
        {
            // Retrieve appointments with necessary navigations: AppointmentType, Doctor, and Doctor.Specializations.
            var appointments = await _unitOfWork.Repository<Appointment>()
                .GetAllAsync(
                    x => x.Doctor.AppUserId == doctorId && x.AppointmentStatus == AppointmentStatus.PendingApproval,
                    includes: ["AppointmentType", "Doctor", "Doctor.Specializations"]);

            if (!appointments.Any())
                return new List<AppointmentResponse>();

            // Extract distinct doctor AppUserIds from the appointments.
            var doctorAppUserIds = appointments
                .Select(a => a.Doctor.AppUserId)
                .Distinct()
                .ToList();

            // Fetch doctor details from the identity database.
            var doctorDetails = await FetchDoctorDetails(doctorAppUserIds, cancellationToken);

            // Map appointments to response DTOs.
            return appointments.Select(a => MapAppointmentResponse(a, doctorDetails)).ToList();
        }
        public async Task<bool> MarkAppointmentAsCompleted(int appointmentId)
        {
            var appointmentRepo = _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>();
            var appointment = await appointmentRepo.GetByIdAsync(appointmentId);

            if (appointment == null || appointment.AppointmentStatus != AppointmentStatus.Confirmed)
            {
                _loggerService.LogWarning($"Completion failed: Appointment {appointmentId} not found or not confirmed.");
                throw new BadRequest("Invalid appointment status.");
            }

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                appointment.AppointmentStatus = AppointmentStatus.Completed;
                appointment.CompletedAt = DateTime.UtcNow;
                appointment.ServiceProvided = true;

                await appointmentRepo.UpdateEntityAsync(appointment);
                await _unitOfWork.CommitAsync();

                //await _notificationService.SendNotificationToPatient(appointment.PatientId, "Appointment Completed",
                //    "Your appointment has been successfully completed.");

                await _unitOfWork.CommitTransactionAsync();
                _loggerService.LogInfo($"Appointment {appointmentId} marked as completed.");
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _loggerService.LogError($"Failed to complete appointment {appointmentId}: {ex.Message}");
                throw;
            }
        }


        public async Task<bool> Pay(int appointmentId, CancellationToken cancellationToken = default)
        {
            // 1️⃣ Retrieve the appointment (including AppointmentType and Payment).
            var appointment = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>()
                .FirstOrDefaultASync(
                    x => x.Id == appointmentId,
                    includes: ["AppointmentType", "Payment"])
                .ConfigureAwait(false);

            if (appointment == null)
                throw new ItemNotFound("Appointment does not exist.");

            if (appointment.AppointmentStatus != AppointmentStatus.PendingPayment)
                throw new BadRequest("Invalid appointment status for payment.");

            // 2️⃣ Determine the amount to charge from the appointment type.
            decimal amountToCharge = appointment.AppointmentType.ConsultationFee;

            // 3️⃣ Generate PaymentIntentId using the PaymentService.
            // If this fails, we choose to fail the operation.
            string paymentIntentId = await _stripeService.GetPaymentIntentId(amountToCharge, appointment.PatientId,appointmentId)
                .ConfigureAwait(false);

            if (string.IsNullOrEmpty(paymentIntentId))
                throw new Exception("Failed to generate PaymentIntent.");

            // 4️⃣ Create a new Payment entity.
            var payment = new Payment
            {
                AppointmentId = appointmentId,
                StripePaymentIntentId = paymentIntentId,
                Amount = amountToCharge,
                Status = PaymentStatus.Paid
            };

            // 5️⃣ Update the appointment's status and associate the new payment.
            appointment.AppointmentStatus = AppointmentStatus.Confirmed;
            appointment.Payment = payment;

            // 6️⃣ Commit changes to the database.
            var rowsAffected = await _unitOfWork.CommitAsync(cancellationToken)
                .ConfigureAwait(false);
            if (rowsAffected <= 0)
                throw new Exception("Payment operation failed.");

            // 7️⃣ Send notifications to patient and doctor.
            // (Assumes _notificationService is injected and configured.)
            //await _notificationService.SendNotificationToPatientAsync(
            //        appointment.PatientId,
            //        $"Your appointment (ID: {appointment.Id}) has been successfully paid and confirmed.",
            //        cancellationToken)
            //    .ConfigureAwait(false);

            //await _notificationService.SendNotificationToDoctorAsync(
            //        appointment.DoctorId,
            //        $"Appointment (ID: {appointment.Id}) with your patient has been confirmed.",
            //        cancellationToken)
            //    .ConfigureAwait(false);

            _loggerService.LogInfo("Appointment {AppointmentId} payment processed successfully.", appointmentId);

            return true;
        }

        public async Task<AppointmentStatus> GetAppointmentStatus(int appointmentId)
        {
            var appointment = await _unitOfWork.Repository<Appointment>().FirstOrDefaultASync(x=> x.Id == appointmentId);

            if (appointment == null)
                throw new ItemNotFound("appointment does not exist");

            return appointment.AppointmentStatus;
        }

        public async Task<bool> CancelAppointmentByDoctor(int appointmentId, string? cancellationReason, CancellationToken cancellationToken = default)
        {
            // 1️⃣ Retrieve the appointment including Payment details.
            var appointment = await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>()
                .FirstOrDefaultASync(x => x.Id == appointmentId, ["Payment"])
                .ConfigureAwait(false);

            if (appointment == null)
            {
                _loggerService.LogWarning("Appointment with ID {AppointmentId} not found.", appointmentId);
                throw new ItemNotFound("Appointment does not exist.");
            }

            // 2️⃣ Validate that the appointment is in a state that can be canceled.
            if (appointment.AppointmentStatus != AppointmentStatus.Confirmed)
            {
                _loggerService.LogWarning("Attempt to cancel appointment with ID {AppointmentId} with invalid status: {Status}.",
                    appointmentId, appointment.AppointmentStatus);
                throw new BadRequest("Invalid appointment: Only confirmed appointments can be canceled by the doctor.");
            }

            // 3️⃣ Set cancellation details.
            appointment.AppointmentStatus = AppointmentStatus.CanceledByDoctor;
            appointment.CancelledAt = DateTimeOffset.UtcNow;
            if (!string.IsNullOrWhiteSpace(cancellationReason))
            {
                appointment.CancellationReason = cancellationReason;
            }

            // 4️⃣ Process payment refund if applicable.
            if (appointment.Payment?.Status == PaymentStatus.Paid)
            {
                appointment.Payment.Status = PaymentStatus.Refunded;
                _loggerService.LogInfo("Appointment {AppointmentId} payment marked as Refunded.", appointmentId);

                // Invoke payment refund service. If the refund fails, fail the operation.
                bool refundSucceeded = await _stripeService.RefundPayment(appointment.Payment.StripePaymentIntentId)
                    .ConfigureAwait(false);
                if (!refundSucceeded)
                {
                    _loggerService.LogError("Refund failed for PaymentIntent {PaymentIntentId} on appointment {AppointmentId}.",
                        appointment.Payment.StripePaymentIntentId, appointmentId);
                    throw new Exception("Failed to process payment refund.");
                }

                // TODO: Optionally, send a notification to the patient regarding the refund.
            }

            // 5️⃣ Update the appointment and commit changes.
            await _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>()
                .UpdateEntityAsync(appointment)
                .ConfigureAwait(false);

            var rowsAffected = await _unitOfWork.CommitAsync()
                .ConfigureAwait(false);

            if (rowsAffected <= 0)
            {
                _loggerService.LogError("Failed to cancel appointment with ID {AppointmentId}.", appointmentId);
                throw new Exception("Failed to cancel appointment.");
            }

            // 6️⃣ Optionally, send a notification to the patient.
            // TODO: Send notification to the patient to inform them of the cancellation.

            _loggerService.LogInfo("Appointment {AppointmentId} successfully canceled by doctor.", appointmentId);
            return true;
        }


        public async Task AutoCancelUnpaidAppointments() // Scheduled with Hangfire
        {
            try
            {
                _loggerService.LogInfo("Started Auto Cancel Unpaid Appointments job.");

                var appointmentRepo = _unitOfWork.GetCustomRepository<AppointmentRepositoryAsync>();

                // Define the criteria for unpaid and expired appointments
                Expression<Func<Appointment, bool>> expression = x =>
                    x.PaymentStatus == PaymentStatus.Pending &&
                    x.AppointmentStatus == AppointmentStatus.PendingPayment &&
                    x.PaymentDueTime < DateTime.UtcNow;

                // Fetch all expired unpaid appointments
                var expiredAppointments = await appointmentRepo.GetAllAsync(expression);

                if (expiredAppointments is null || !expiredAppointments.Any())
                {
                    _loggerService.LogInfo("No expired unpaid appointments found.");
                    return;
                }

             //   Prepare notifications
                var notificationTasks = new List<Task>();

                foreach (var appointment in expiredAppointments)
                {
                    appointment.AppointmentStatus = AppointmentStatus.AutoCanceled;
                    appointment.CancelledAt = DateTime.UtcNow;
                    appointment.CancellationReason = "Payment not completed within the required timeframe.";

                    _loggerService.LogWarning($"Auto-canceled appointment (ID: {appointment.Id}) due to unpaid status.");

                    // 🔹 Notify the patient about auto-cancelation
                    //if (!string.IsNullOrEmpty(appointment.PatientDeviceToken)) // Ensure the device token exists
                    //{
                    //    notificationTasks.Add(_firebaseNotificationService.SendPushNotification(
                    //        appointment.PatientDeviceToken,
                    //        "Appointment Auto-Canceled",
                    //        $"Your appointment on {appointment.StartDate:MMM dd, yyyy} at {appointment.StartDate:hh:mm tt} was auto-canceled due to non-payment."
                    //    ));
                    //}
                }

                // 🔹 Perform batch update for better efficiency
                await appointmentRepo.UpdateRangeAsync(expiredAppointments);
                await _unitOfWork.CommitAsync();

                // 🔹 Send notifications asynchronously
                await Task.WhenAll(notificationTasks);

                _loggerService.LogInfo($"Successfully auto-canceled {expiredAppointments.Count} unpaid appointments.");
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"AutoCancelUnpaidAppointments job failed: {ex.Message}", ex);
                throw;
            }
        }


        private async Task<IEnumerable<Appointment>> FetchPatientAppointments(int userId, AppointmentStatus? status)
        {
            return await _unitOfWork.Repository<Appointment>()
                .GetAllAsync(
                    x => x.PatientId == userId && (status == null || x.AppointmentStatus == status),
                    ["AppointmentType", "Doctor", "Doctor.Specializations"])
                .ConfigureAwait(false);
        }

        private async Task<IEnumerable<Appointment>> FetchDoctorAppointments(int doctorId, AppointmentStatus? status)
        {
            return await _unitOfWork.Repository<Appointment>()
                .GetAllAsync(
                    x => x.Doctor.AppUserId == doctorId && (status == null || x.AppointmentStatus == status),
                    ["AppointmentType", "Doctor", "Doctor.Specializations"])
                .ConfigureAwait(false);
        }

        private async Task<Dictionary<int, DoctorDetails>> FetchDoctorDetails(List<int> doctorAppUserIds,CancellationToken cancellationToken)
        {
            return await _userManager.Users
                .Where(u => doctorAppUserIds.Contains(u.Id))
                .Select(u => new DoctorDetails
                {
                    Id = u.Id,
                    FirstName = u.FirstName ?? "Unknown",
                    LastName = u.LastName ?? "Unknown",
                    ImagePath = u.ImagePath ?? string.Empty
                })
                .ToDictionaryAsync(u => u.Id, cancellationToken);
        }


        private AppointmentResponse MapAppointmentResponse(Appointment appointment, Dictionary<int, DoctorDetails> doctorDetails)
        {
            var doctor = doctorDetails.GetValueOrDefault(appointment.Doctor.AppUserId) ?? new DoctorDetails
            {
                Id = appointment.Doctor.AppUserId,
                FirstName = "Unknown",
                LastName = "Unknown",
                ImagePath = string.Empty
            };

            return new AppointmentResponse
            {
                Id = appointment.Id.ToString(),
                StartDate = appointment.StartDate,
                EndDate = appointment.EndDate,
                AppointmentStatus = appointment.AppointmentStatus,
                AppointmentType = _mapper.Map<AppointmentTypeResponse>(appointment.AppointmentType),
                DoctorId = appointment.Doctor.Id,
                DoctorFullName = $"{doctor.FirstName} {doctor.LastName}",
                DoctorImage = !string.IsNullOrEmpty(doctor.ImagePath)
                                ? $"{_baseUrl}{doctor.ImagePath}"
                                : $"{_baseUrl}default.jpg",
                DoctorSpecialization = _mapper.Map<List<SpecializationResponse>>(appointment.Doctor.Specializations ?? new List<Specialization>())
            };
        }
    }
}

