namespace MosefakApp.Domains.Entities
{
    public class Appointment : BaseEntity
    {
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public int PatientId { get; set; } // fk for AppUser to represent Patient and didn't put navigation because it's exist in another DB
        
        public DateTimeOffset StartDate { get; set; } 
        public DateTimeOffset EndDate { get; set; }
        
        public int AppointmentTypeId { get; set; }
        public AppointmentType AppointmentType { get; set; } = null!;
     
        public string? ProblemDescription { get; set; } 
        public AppointmentStatus AppointmentStatus { get; set; } = AppointmentStatus.PendingApproval;
        public string? CancellationReason { get; set; } // will provide if the AppointmentStatus is set to Cancelled
       
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending; // for Payment
        public Payment? Payment { get; set; }
        public DateTimeOffset? PaymentDueTime { get; set; } // Payment deadline
        
        public DateTimeOffset? ConfirmedAt { get; set; }
        public DateTimeOffset? CancelledAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
       
        public bool ApprovedByDoctor { get; set; } = false;
        public bool ServiceProvided { get; set; } = false;
    }
}
