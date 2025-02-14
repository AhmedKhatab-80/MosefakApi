namespace MosefakApp.Core.IServices.Stripe
{
    public interface IStripeService
    {
        Task<string> GetPaymentIntentId(decimal amount, int appUserId, int appointmentId);
        Task<bool> RefundPayment(string paymentIntentId);
    }
}
