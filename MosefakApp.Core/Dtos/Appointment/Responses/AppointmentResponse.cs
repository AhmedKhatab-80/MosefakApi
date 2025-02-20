namespace MosefakApp.Core.Dtos.Appointment.Responses
{
    public class AppointmentResponse 
    {
        public int Id { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
        public int DoctorId { get; set; }
        public string DoctorFullName { get; set; } = null!;
        public string? DoctorImage { get; set; }
        public IList<SpecializationResponse> DoctorSpecialization { get; set; } = null!;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public AppointmentTypeResponse AppointmentType { get; set; } = null!;
    }
}
