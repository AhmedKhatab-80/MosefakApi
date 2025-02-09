namespace MosefakApp.Core.IServices.Cache
{
    public interface ICacheService
    {
        Task<string> GetCachedResponseAsync(string key);
        Task CacheResponseAsync(string key, object response, TimeSpan duration);
        Task RemoveCachedResponseAsync(string key);
    }
}
