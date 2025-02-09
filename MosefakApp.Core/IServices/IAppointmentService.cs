namespace MosefakApp.Core.IServices
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentResponse>?> UpcomingAppointments(int userIdFromClaims);
        Task<IEnumerable<AppointmentResponse>?> CanceledAppointments(int userIdFromClaims);
        Task<IEnumerable<AppointmentResponse>?> CompletedAppointments(int userIdFromClaims);
        Task<bool> CancelAppointmentAsync(int appointmentId, string? cancelationReason);
        Task<bool> RescheduleAppointmentAsync(int appointmentId, DateTime newDateTime);
        Task<bool> IsTimeSlotAvailable(int doctorId, DateTime startDate, DateTime endDate);
        Task<bool> BookAppointment(BookAppointmentRequest request,int appUserIdFromClaims);
    }
}
