namespace MosefakApi.Business.Services.Logging
{
    public class LoggerService : ILoggerService
    {
        private readonly ILogger<LoggerService> _logger;

        public LoggerService(ILogger<LoggerService> logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message, params object[]? parameters)
        {
            _logger.LogInformation(message, parameters!);
        }

        public void LogWarning(string message, params object[]? parameters)
        {
            _logger.LogWarning(message, parameters!);
        }

        public void LogDebug(string message, params object[]? parameters)
        {
            _logger.LogDebug(message, parameters!);
        }

        public void LogError(string message, params object[]? parameters)
        {
            _logger.LogError(message, parameters!);
        }
    }
}
