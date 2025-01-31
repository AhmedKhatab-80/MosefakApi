namespace MosefakApp.Core.Dtos.Doctor.Responses
{
    public class DoctorDetails
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
    }
}
