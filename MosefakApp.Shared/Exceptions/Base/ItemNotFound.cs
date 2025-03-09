namespace MosefakApp.Shared.Exceptions.Base
{
    public class ItemNotFound : BaseException
    {
        public ItemNotFound() : base("Item Not Found") { }

        public ItemNotFound(string? message) : base(message)
        {

        }

        public ItemNotFound(int StatusCode) : base(StatusCode)
        {
        }

        public ItemNotFound(int StatusCode, string? message) : base(StatusCode, message)
        {
        }

        public ItemNotFound(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
