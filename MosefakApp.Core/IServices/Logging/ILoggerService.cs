namespace MosefakApp.Core.IServices.Logging
{
    public interface ILoggerService
    {
        void LogInfo(string message, params object[]? args);
        void LogWarning(string message, params object[]? args);
        void LogError(string message, params object[]? args);
        void LogDebug(string message, params object[]? args);
    }

}
