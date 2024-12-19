namespace MosefakApp.API.MiddleWares.Models
{
    public class CustomResponse // make Msg and Details as Nullable, because may not fill it and enought with statusCode.
    {
        public CustomResponse()
        {

        }
        public CustomResponse(int statusCode, string? message, string? details)
        {
            StatusCode = statusCode;
            Message = message ?? GenerateDefaultMessage(statusCode);
            Details = details;
        }

        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? Details { get; set; }

        public override string ToString()
        {
            return $"Status Code: {StatusCode}\n" +
                   $"Message: {Message}\n" +
                   $"Details: {Details}\n";
        }


        private string? GenerateDefaultMessage(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return "Resourse Not Found";
                case 400:
                    return "Bad Request";
                case 401:
                    return "UnAuthorized";
                case 405:
                    return "Method Not allowed";
                case 409:
                    return "Resourse Already Exist";
                default:
                    return "Internal server Error";
            }
        }

    }
}
