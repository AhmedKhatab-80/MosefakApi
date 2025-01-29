namespace MosefakApp.Core.Dtos.ClinicAddress.Responses
{
    public class ClinicAddressResponse
    {
        public int Id { get; set; }
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
    }
}
