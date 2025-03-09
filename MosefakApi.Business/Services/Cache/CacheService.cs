namespace MosefakApi.Business.Services.Cache
{
    public class CacheService : ICacheService 
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task CacheResponseAsync(string key, object response, TimeSpan duration)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var serilizedResponse = System.Text.Json.JsonSerializer.Serialize(response, jsonOptions);

            _memoryCache.Set(key, serilizedResponse, duration);

            await Task.CompletedTask;
        }

        public async Task<string> GetCachedResponseAsync(string key)
        {
            _memoryCache.TryGetValue(key, out string cachedResponse);

            return await Task.FromResult(cachedResponse!);
        }

        public Task RemoveCachedResponseAsync(string key)
        {
            _memoryCache.Remove(key);

            return Task.CompletedTask;
        }
    }
}
