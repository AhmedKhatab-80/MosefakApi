namespace MosefakApp.Core.IServices.Stripe
{
    public interface IStripeService
    {
        Task<(string paymentIntentId,string clientSecret)> GetPaymentIntentId(decimal amount, string appUserId, string appointmentId);
        Task<bool> RefundPayment(string paymentIntentId);
        Task<string> VerifyPaymentStatus(string paymentIntentId);
    }
}
