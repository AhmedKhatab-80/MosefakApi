namespace MosefakApp.API.MiddleWares
{
    public class ErrorHandlingMiddleWare
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // as Log
                await HandleError(ex, context);
            }
        }

        private async Task HandleError(Exception ex, HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new CustomResponse()
            {
                Message = ex.Message,
            };

            switch (ex)
            {
                case ItemNotFound:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case BadRequest:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case ItemAlreadyExist:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    break;
                case InvalidOperation:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                default:
                    break;
            }

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}
