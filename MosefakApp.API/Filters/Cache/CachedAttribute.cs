namespace MosefakApp.API.Filters.Cache
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _durationToLiveInMemoryBySeconds;

        public CachedAttribute(int duration)
        {
            _durationToLiveInMemoryBySeconds = duration;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // ask CLR to create obj from CacheService

            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

            var cachedKey = GenerateCachedKey(context.HttpContext.Request);

            var cachedResponse = await cacheService.GetCachedResponseAsync(cachedKey);

            if (cachedResponse != null)
            {
                var contentResult = new ContentResult()
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                context.Result = contentResult;

                return;
            }
            else
            {
                var executedEndpointContext = await next();

                if (executedEndpointContext.Result is OkObjectResult objectResult)
                {
                    await cacheService.CacheResponseAsync(cachedKey, objectResult.Value!, TimeSpan.FromSeconds(_durationToLiveInMemoryBySeconds));
                }
            }

        }

        private string GenerateCachedKey(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append(request.Path); // /api/Products

            if (request.Query.Count > 0)
            {
                foreach (var (key, value) in request.Query.OrderBy(p => p.Key))
                {
                    keyBuilder.Append($"|{key}-{value}");
                }
            }

            return keyBuilder.ToString();
        }
    }
}
