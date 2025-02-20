namespace MosefakApi.Business.Services.Stripe
{
    public class StripeService : IStripeService
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerService _logger;

        public StripeService(IConfiguration configuration, ILoggerService logger)
        {
            _configuration = configuration;
            _logger = logger;
            StripeConfiguration.ApiKey = _configuration["PaymentSettings:SecretKey"];
        }

        public async Task<string> GetPaymentIntentId(decimal amount, int appUserId, int appointmentId)
        {
            try
            {
                // Stripe requires amount in the smallest currency unit (e.g., cents for USD)
                long amountInCents = (long)(amount * 100);

                var options = new PaymentIntentCreateOptions
                {
                    Amount = amountInCents,
                    Currency = "usd", // You might read this from configuration
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "appUserId", appUserId.ToString() },
                        { "appointmentId", appointmentId.ToString() }
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);
                return paymentIntent.Id;
            }
            catch (StripeException ex)
            {
                _logger.LogError("Stripe error generating PaymentIntent for appointment {AppointmentId} because {ex.Message}.", appointmentId,ex.Message);
                // Fail the operation if PaymentIntent cannot be generated.
                throw new Exception("Payment processing error.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error generating PaymentIntent for appointment {AppointmentId} because {ex.Message}.", appointmentId, ex.Message);
                throw;
            }
        }

        public async Task<bool> RefundPayment(string paymentIntentId)
        {
            try
            {
                var options = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId
                };

                var service = new RefundService();
                var refund = await service.CreateAsync(options);

                // Check if the refund succeeded.
                return string.Equals(refund.Status, "succeeded", StringComparison.OrdinalIgnoreCase);
            }
            catch (StripeException ex)
            {
                _logger.LogError("Stripe error processing refund for PaymentIntent {PaymentIntentId} because {ex.Message}.", paymentIntentId, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error processing refund for PaymentIntent {PaymentIntentId} because {ex.Message}.", paymentIntentId, ex.Message);
                return false;
            }
        }
    }
}
