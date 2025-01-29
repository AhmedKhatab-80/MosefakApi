namespace MosefakApp.Domains.Entities
{
    public class Doctor : BaseEntity
    {
        public int AppUserId { get; set; } // to get another info like(email,phone,age,etc) from another database by using AppUserId
        public int YearOfExperience { get; set; }
        public string LicenseNumber { get; set; } = null!;
        public IList<ClinicAddress> ClinicAddresses { get; set; } = new List<ClinicAddress>(); 
        public string AboutMe { get; set; } = null!;
        public IList<Specialization> Specializations { get; set; } = new List<Specialization>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<WorkingTime> WorkingTimes { get; set; } = new List<WorkingTime>();
        public int NumberOfReviews { get; set; } // denormlized property for best performance  
        public decimal ConsultationFee { get; set; } // Fee per appointment
    }
}
