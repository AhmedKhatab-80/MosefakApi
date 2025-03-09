namespace MosefakApp.Core.Dtos.Clinic.Requests
{
    public class ClinicRequest // will Take [FromForm]
    {
        public string Name { get; set; } = null!;       // e.g., "Sunrise Medical Center"

        // Address Fields
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string ApartmentOrSuite { get; set; } = null!; // e.g., "Building 3, Suite 450"
        public string Landmark { get; set; } = null!;   // e.g., "Next to Union Square"

        // Optional Media Fields
        public IFormFile? LogoPath { get; set; }

        public IFormFile? ClinicImage { get; set; }

        // Contact Field
        public string PhoneNumber { get; set; } = null!;
        public List<WorkingTimeRequest> WorkingTimes { get; set; } = new List<WorkingTimeRequest>();
    }
}
