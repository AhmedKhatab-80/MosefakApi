namespace MosefakApp.Core.Dtos.Appointment.Requests
{
    public class CancelAppointmentRequest
    {
        public int AppointmentId { get; set; }
        public string? CancelationReason { get; set; }
    }
}

