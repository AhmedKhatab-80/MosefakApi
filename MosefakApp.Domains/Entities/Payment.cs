namespace MosefakApp.Domains.Entities
{
    public class Payment : BaseEntity
    {
        public int AppointmentId { get; set; }
        public Guid TransactionId { get; set; } = Guid.NewGuid();
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string StripePaymentIntentId { get; set; } = null!; // For Stripe Payment Tracking
        public Appointment Appointment { get; set; } = null!;
    }
}
