namespace MosefakApp.Core.Dtos.Appointment.Requests
{
    public class BookAppointmentRequest
    {
        public int DoctorId { get; set; }
        public DateTimeOffset StartDate { get; set; } 
        public string? ProblemDescription { get; set; }
        public string AppointmentTypeId { get; set; } = null!;
    }
}

