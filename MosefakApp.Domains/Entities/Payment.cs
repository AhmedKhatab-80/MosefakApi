namespace MosefakApp.Domains.Entities
{
    public class Payment : BaseEntity
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd"; // Default currency
        public PaymentMethod PaymentMethod { get; set; } // E.g., Card, Wallet
        public DateTime PaymentDate { get; set; }
        public string TransactionId { get; set; } = null!; // Unique identifier for completed payment
        public bool IsSuccessful { get; set; }

        // Stripe-specific fields
        public string PaymentIntentId { get; set; } = null!; // Stripe's unique ID for the payment
        public string ClientSecret { get; set; } = null!; // Used to confirm payment on the frontend

        // Relationships
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;

        public int PatientId { get; set; } // AppUser Id
    }

}
