namespace MosefakApp.Domains.Entities
{
    public class Appointment : BaseEntity
    {
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        public int AppUserId { get; set; } // fk for AppUser to represent Patient and didn't put navigation because it's exist in another DB
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
        public string ProblemDescription { get; set; } = null!;
        public AppointmentStatus AppointmentStatus { get; set; } = AppointmentStatus.Scheduled;
        public string? CancellationReason { get; set; }
        public bool IsPaid { get; set; } // for Payment
        public Payment Payment { get; set; } = null!; 
    }
}
