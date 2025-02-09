namespace MosefakApp.Domains.Entities
{
    public class AppointmentType : BaseEntity
    {
        public TimeOnly Duration { get; set; }
        public string VisitType { get; set; } = null!;
        public decimal ConsultationFee { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
