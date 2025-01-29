namespace MosefakApp.Domains.Entities
{
    public class ClinicAddress : BaseEntity 
    {
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
    }
}
