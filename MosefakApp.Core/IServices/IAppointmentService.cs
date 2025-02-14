namespace MosefakApp.Core.IServices
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentResponse>?> UpcomingAppointments(int userIdFromClaims);
        Task<IEnumerable<AppointmentResponse>?> CanceledAppointments(int userIdFromClaims);
        Task<IEnumerable<AppointmentResponse>?> CompletedAppointments(int userIdFromClaims);
        Task<bool> RescheduleAppointmentAsync(int appointmentId, DateTimeOffset newDateTime);
        Task<bool> IsTimeSlotAvailable(int doctorId, DateTimeOffset startDate, DateTimeOffset endDate);
        Task<bool> ApproveAppointmentByDoctor(int appointmentId);
        Task<bool> RejectAppointmentByDoctor(int appointmentId, string? rejectionReason); 
        Task<bool> BookAppointment(BookAppointmentRequest request,int appUserIdFromClaims);
        Task<bool> Pay(int appointmentId);
        Task<bool> MarkAppointmentAsCompleted(int appointmentId);
        Task<bool> CancelAppointmentByPatient(int appointmentId, string? cancelationReason);
        Task<bool> CancelAppointmentByDoctor(int appointmentId, string? cancelationReason); 
        Task AutoCancelUnpaidAppointments(); // by using HanjFire
    }
}
