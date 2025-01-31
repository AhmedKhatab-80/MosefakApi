namespace MosefakApp.Core.IServices
{
    public interface IAppointmentService
    {
        // Note, Upcoming, canceled, completed will return in response (DoctorName,DoctorImage,DoctorSpeciality,Date,Time)

        // Upcoming Appointments for patient which userIdFromClaims

        // canceled Appointments for patient which userIdFromClaims

        // completed Appointments for patient which userIdFromClaims

        // Cancel Appointment

        // Reschedule Appointment

        // Cancel an appointment
        Task<bool> CancelAppointmentAsync(int appointmentId);

        // Reschedule an appointment
        Task<bool> RescheduleAppointmentAsync(int appointmentId, DateTime newDateTime);
    }
}
