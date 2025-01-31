namespace MosefakApp.Core.Dtos.Appointment.Requests
{
    public class RescheduleAppointmentRequest
    {
        public int AppointmentId { get; set; }
        public DateTime NewDateTime { get; set; }
    }
}

