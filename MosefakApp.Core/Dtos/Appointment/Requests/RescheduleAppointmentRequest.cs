namespace MosefakApp.Core.Dtos.Appointment.Requests
{
    public class RescheduleAppointmentRequest
    {
        public string AppointmentId { get; set; } = null!;
        public DateTime selectedDate { get; set; }
        public TimeSlot newTimeSlot { get; set; } = null!;
    }
}

