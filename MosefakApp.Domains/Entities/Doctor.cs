namespace MosefakApp.Domains.Entities
{
    public class Doctor : BaseEntity
    {
        public int AppUserId { get; set; } // to get another info like(email,phone,age,etc) from another database by using AppUserId
        public int YearOfExperience { get; set; }
        public string LicenseNumber { get; set; } = null!;
        public string AboutMe { get; set; } = null!;
        public List<ClinicAddress> ClinicAddresses { get; set; } = new List<ClinicAddress>(); 
        public List<Specialization> Specializations { get; set; } = new List<Specialization>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public List<WorkingTime> WorkingTimes { get; set; } = new List<WorkingTime>();
        public int NumberOfReviews { get; set; } = 0; // denormlized property for best performance  
        public List<AppointmentType> AppointmentTypes { get; set; } = new List<AppointmentType>();
    }
}
