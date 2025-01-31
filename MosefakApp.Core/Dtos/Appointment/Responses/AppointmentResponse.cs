namespace MosefakApp.Core.Dtos.Appointment.Responses
{
    public class AppointmentResponse 
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DoctorFullName { get; set; } = null!;
        public string? DoctorImage { get; set; }
        public IList<SpecializationResponse> DoctorSpecialization { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AppointmentType AppointmentType { get; set; } 
    }
}
