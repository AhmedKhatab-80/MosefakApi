namespace MosefakApp.Core.Dtos.Appointment.Requests
{
    public class BookAppointmentRequest
    {
        public int DoctorId { get; set; }
        public DateTime StartDate { get; set; }
        public string ProblemDescription { get; set; } = null!;
        public int AppointmentTypeId { get; set; }
    }
}

