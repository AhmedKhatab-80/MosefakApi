namespace MosefakApp.API.MiddleWares.Models
{
    public class ValidationErrorCustomResponse : CustomResponse
    {
        public IList<string> Errors { get; set; }
        public ValidationErrorCustomResponse() : base(400, null, null)
        {
            Errors = new List<string>();
        }

    }
}
