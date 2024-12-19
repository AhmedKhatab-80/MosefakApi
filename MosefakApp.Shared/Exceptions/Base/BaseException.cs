namespace MosefakApp.Shared.Exceptions.Base
{
    public class BaseException : ApplicationException
    {
        public int StatusCode { get; set; }
        public BaseException() { }

        public BaseException(string? message) : base(message)
        {

        }

        public BaseException(int statusCode)
        {
            StatusCode = statusCode;
        }

        public BaseException(int statusCode, string? message) : base(message)
        {
            StatusCode = statusCode;
        }

        public BaseException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
