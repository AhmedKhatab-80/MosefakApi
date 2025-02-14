namespace MosefakApp.Domains.Entities
{
    public class Clinic : BaseEntity
    {
        public string Name { get; set; } = null!;       // e.g., "Sunrise Medical Center"

        // Address Fields
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string ApartmentOrSuite { get; set; } = null!; // e.g., "Building 3, Suite 450"
        public string Landmark { get; set; } = null!;   // e.g., "Next to Union Square"

        // Optional Media Fields
        public string? LogoPath { get; set; }    

        public string? ClinicImage { get; set; }    

        // Contact Field
        public string PhoneNumber { get; set; } = null!;

        // Relationships
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        public ICollection<WorkingTime> WorkingTimes { get; set; } = new HashSet<WorkingTime>();
    }
}
