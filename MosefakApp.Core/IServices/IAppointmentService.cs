namespace MosefakApp.Core.IServices
{
    public interface IAppointmentService
    {
        // 🔹 Patient Methods
        Task<List<AppointmentResponse>> GetPatientAppointments(int patientId, AppointmentStatus? status = null, CancellationToken cancellationToken = default);
        Task<AppointmentResponse> GetAppointmentById(int appointmentId, CancellationToken cancellationToken = default);
        Task<List<AppointmentResponse>> GetAppointmentsByDateRange(int userId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
        Task<bool> RescheduleAppointmentAsync(int appointmentId, DateTime selectedDate, TimeSlot newTimeSlot); 
        Task<bool> CancelAppointmentByPatient(int patientId,int appointmentId, string? cancelationReason, CancellationToken cancellationToken = default); 
        Task<bool> BookAppointment(BookAppointmentRequest request, int appUserId, CancellationToken cancellationToken = default); 
        Task<AppointmentStatus> GetAppointmentStatus(int appointmentId); // Used by: Patients, Doctors, Admins, Background Jobs (Hangfire)
        Task<bool> Pay(int appointmentId, CancellationToken cancellationToken = default); 

        // 🔹 Doctor Methods
        Task<List<AppointmentResponse>> GetDoctorAppointments(int doctorId, AppointmentStatus? status = null, CancellationToken cancellationToken = default);
        Task<List<AppointmentResponse>> GetPendingAppointmentsForDoctor(int doctorId, CancellationToken cancellationToken = default); 
        Task<bool> ApproveAppointmentByDoctor(int appointmentId);
        Task<bool> RejectAppointmentByDoctor(int appointmentId, string? rejectionReason);
        Task<bool> CancelAppointmentByDoctor(int appointmentId, string? cancelationReason, CancellationToken cancellationToken = default);
        Task<bool> MarkAppointmentAsCompleted(int appointmentId);
        Task<List<AppointmentResponse>> GetAppointmentsByDateRangeForDoctor(int doctorId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);

        // 🔹 Utility Methods
        Task AutoCancelUnpaidAppointments(); // Handled by Hangfire
    }

}
