namespace MosefakApp.Core.Dtos.Doctor.Responses
{
    public class DoctorDto // for Top Ten Doctors, I don't need all data that in DoctorResponse, so made DoctorDto
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
        public int YearOfExperience { get; set; }
        public int NumberOfReviews { get; set; }
        public IList<SpecializationResponse> Specializations { get; set; } = new List<SpecializationResponse>();
        public List<AppointmentTypeResponse> AppointmentTypes { get; set; } = new List<AppointmentTypeResponse>();
        public IList<ReviewResponse> Reviews { get; set; } = new List<ReviewResponse>();
    }
}
