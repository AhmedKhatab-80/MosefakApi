namespace MosefakApp.Domains.Entities
{
    public class Appointment : BaseEntity
    {
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        public int AppUserId { get; set; } // fk for AppUser to represent Patient and didn't put navigation because it's exist in another DB
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
        public int AppointmentTypeId { get; set; }
        public AppointmentType AppointmentType { get; set; } = null!;
        public string ProblemDescription { get; set; } = null!;
        public AppointmentStatus AppointmentStatus { get; set; } = AppointmentStatus.Scheduled;
        public string? CancellationReason { get; set; } // will provide if the AppointmentStatus is set to Cancelled
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending; // for Payment
        public Payment? Payment { get; set; } 
    }
}
