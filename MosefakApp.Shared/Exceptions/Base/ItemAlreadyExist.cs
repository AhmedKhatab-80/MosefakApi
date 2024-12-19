namespace MosefakApp.Shared.Exceptions.Base
{
    public class ItemAlreadyExist : BaseException
    {
        public ItemAlreadyExist() : base("Item Already Exist") { }

        public ItemAlreadyExist(string? message) : base(message)
        {

        }

        public ItemAlreadyExist(int StatusCode) : base(StatusCode)
        {
        }

        public ItemAlreadyExist(int StatusCode, string? message) : base(StatusCode, message)
        {
        }

        public ItemAlreadyExist(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
