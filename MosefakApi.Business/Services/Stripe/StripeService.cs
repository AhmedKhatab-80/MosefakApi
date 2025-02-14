namespace MosefakApi.Business.Services.Stripe
{
    public class StripeService : IStripeService
    {
        private readonly IConfiguration _configuration;

        public StripeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetPaymentIntentId(decimal amount, int appUserId, int appointmentId)
        {
            StripeConfiguration.ApiKey = _configuration["PaymentSettings:SecretKey"];

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Amount in cents
                Currency = "usd",
                Metadata = new Dictionary<string, string>
                {
                   { "appointmentId", appointmentId.ToString() },
                   { "userId", appUserId.ToString() }
                }
            };

            var service = new PaymentIntentService();

            try
            {
                var paymentIntent = await service.CreateAsync(options);
               
                return paymentIntent.Id;
            }
            catch (StripeException ex)
            {
                // Log the exception and rethrow or return a meaningful response
                throw new ApplicationException("Failed to create payment intent", ex);
            }
        }

        public Task<bool> RefundPayment(string paymentIntentId)
        {
            throw new NotImplementedException();
        }
    }
}
