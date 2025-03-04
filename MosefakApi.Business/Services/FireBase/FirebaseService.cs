namespace MosefakApi.Business.Services.FireBase
{
    public class FirebaseService : IFirebaseService
    {
        private readonly string _serverKey;
        private readonly HttpClient _httpClient;
        private readonly ILoggerService _logger; // ✅ Logging

        public FirebaseService(IConfiguration configuration, HttpClient httpClient, ILoggerService logger)
        {
            _serverKey = configuration["Firebase:ServerKey"]!;
            _httpClient = httpClient; // ✅ Injected `HttpClient` instead of new instance
            _logger = logger;
        }

        public async Task<bool> SendNotificationAsync(string fcmToken, string title, string message, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fcmToken))
            {
                _logger.LogWarning("FCM token is missing. Notification not sent.");
                return false;
            }

            var url = "https://fcm.googleapis.com/fcm/send";
            var payload = new
            {
                to = fcmToken,
                notification = new
                {
                    title,
                    body = message
                }
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Headers = { { "Authorization", $"key={_serverKey}" } },
                Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
            };

            try
            {
                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"🔥 Firebase Notification Failed: {errorResponse} (Status Code: {response.StatusCode})");
                    return false;
                }

                _logger.LogInfo($"✅ Firebase Notification Sent Successfully to {fcmToken}");
                return true;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError($"🚨 HTTP Request Error while sending Firebase notification: {httpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Unexpected Error while sending Firebase notification: {ex.Message}");
                return false;
            }
        }
    }

}
