namespace MosefakApp.Shared.Exceptions.Base
{
    public class BadRequest : BaseException
    {
        public BadRequest() : base("Bad Request") { }

        public BadRequest(string? message) : base(message)
        {

        }

        public BadRequest(int StatusCode) : base(StatusCode)
        {
        }

        public BadRequest(int StatusCode, string? message) : base(StatusCode, message)
        {
        }

        public BadRequest(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
