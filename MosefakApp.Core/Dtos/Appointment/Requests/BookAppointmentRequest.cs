namespace MosefakApp.Core.Dtos.Appointment.Requests
{
    public class BookAppointmentRequest
    {
        public string DoctorId { get; set; } = null!;
        public DateTimeOffset StartDate { get; set; } 
        public string? ProblemDescription { get; set; }
        public string AppointmentTypeId { get; set; } = null!;
    }
}

