namespace MosefakApp.API.MiddleWares
{
    public class CalculateTimeOfRequest
    {
        private readonly RequestDelegate _next;

        public CalculateTimeOfRequest(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var watch = new Stopwatch();
                watch.Start();
                // make logic here on request before move it to next middleware.
                await _next(context);
                watch.Stop();
                Console.WriteLine($"Time of Request is: {watch.ElapsedMilliseconds} MS");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
