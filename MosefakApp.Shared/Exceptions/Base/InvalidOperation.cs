namespace MosefakApp.Shared.Exceptions.Base
{
    public class InvalidOperation : BaseException
    {
        public InvalidOperation() : base("Invalid Operation") { }

        public InvalidOperation(string? message) : base(message)
        {

        }

        public InvalidOperation(int StatusCode) : base(StatusCode)
        {
        }

        public InvalidOperation(int StatusCode, string? message) : base(StatusCode, message)
        {
        }

        public InvalidOperation(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
