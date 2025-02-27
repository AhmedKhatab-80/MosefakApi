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

        /// <summary>
        /// Creates a PaymentIntent and returns its ID and ClientSecret.
        /// </summary>
        public async Task<(string paymentIntentId, string clientSecret)> GetPaymentIntentId(decimal amount, string appUserId, string appointmentId)
        {
            try
            {
                long amountInCents = (long)(amount * 100); // Convert amount to cents

                var options = new PaymentIntentCreateOptions
                {
                    Amount = amountInCents,
                    Currency = "usd", // Fetch from config if needed
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
                {
                    { "appUserId", appUserId },
                    { "appointmentId", appointmentId }
                }
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                return (paymentIntent.Id, paymentIntent.ClientSecret);
            }
            catch (StripeException ex)
            {
                _logger.LogError("Stripe error generating PaymentIntent for appointment {AppointmentId}: {ErrorMessage}", appointmentId, ex.Message);
                throw new Exception("Payment processing error.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error generating PaymentIntent for appointment {AppointmentId}: {ErrorMessage}", appointmentId, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Verifies the status of a PaymentIntent using its ID.
        /// </summary>
        public async Task<string> VerifyPaymentStatus(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentIntentId);

                _logger.LogInfo("Verified payment for {PaymentIntentId}: Status = {Status}", paymentIntentId, paymentIntent.Status);

                return paymentIntent.Status; // Will return "succeeded", "requires_payment_method", etc.
            }
            catch (Exception ex)
            {
                _logger.LogError("Error verifying payment {PaymentIntentId}: {Message}", paymentIntentId, ex.Message);
                return "error";
            }
        }


        /// <summary>
        /// Processes a refund for a given PaymentIntent.
        /// </summary>
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

                _logger.LogInfo("Refund processed for PaymentIntent {PaymentIntentId}: Status = {Status}",
                    paymentIntentId, refund.Status);

                return string.Equals(refund.Status, "succeeded", StringComparison.OrdinalIgnoreCase);
            }
            catch (StripeException ex)
            {
                _logger.LogError("Stripe refund failed: {ErrorMessage}, Stripe Code: {ErrorCode}",
                    ex.Message, ex.StripeError.Code);

                throw new Exception($"Refund failed: {ex.StripeError.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error processing refund for PaymentIntent {PaymentIntentId}: {ErrorMessage}",
                    paymentIntentId, ex.Message);
                return false;
            }
        }

    }
}
