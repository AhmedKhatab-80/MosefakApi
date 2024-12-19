namespace MosefakApi.Business.Services.Cache
{
    public class CacheService : ICacheService 
    {
        private readonly IDatabase _database;

        public CacheService(IConnectionMultiplexer multiplexer)
        {
            _database = multiplexer.GetDatabase();
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan duration)
        {
            if (response is null)
                return;

            var jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // For Front-end
            };

            await _database.StringSetAsync(cacheKey, JsonSerializer.Serialize(response, jsonOptions), duration);
        }

        public async Task<string?> GetCachedResponse(string cacheKey)
        {
            var response = await _database.StringGetAsync(cacheKey);

            if (response.IsNullOrEmpty)
                return null;

            return response;
        }

        public Task RemoveCacheKey(string cacheKey)
        {
            throw new NotImplementedException();
        }
    }
}
