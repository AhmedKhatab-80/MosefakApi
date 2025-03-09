namespace MosefakApp.Domains.Entities
{
    public class Experience : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string? HospitalLogo { get; set; } 
        public string HospitalName { get; set; } = null!;
        public string Location { get; set; } = null!;
        public EmploymentType EmploymentType { get; set; }
        public string? JobDescription { get; set; } 
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; } // I made it nullable because it's if still working here will be null...
        public bool CurrentlyWorkingHere { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
    }
}
