namespace MosefakApp.Core.Dtos.User.Responses
{
    public class AddressUserResponse
    {
        public int Id { get; set; }
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
    }
}
